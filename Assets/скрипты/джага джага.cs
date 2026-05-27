using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Компоненты")]
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Настройки движения")]
    public float speed = 12f;
    public float gravity = -35f; // Сильная гравитация для четкого управления
    public float jumpHeight = 2.5f;
    [Range(0, 1)] public float airControl = 0.6f;

    [Header("Прыжки")]
    public int maxJumps = 2;
    private int jumpCount;

    [Header("Дэш (Shift)")]
    public float dashSpeed = 35f;
    public float dashTime = 0.15f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    [Header("Wall Jump (Тег: WallJump)")]
    public float wallJumpForce = 12f;
    public float wallJumpUpForce = 10f;
    public float wallCheckDistance = 0.8f;

    // --- НАСТРОЙКИ АУДИО ---
    [Header("Аудио: Стрельба")]
    public AudioSource weaponAudioSource;
    public float weaponFadeSpeed = 5f;        
    public float timeToCutFromEnd = 0.3f;
    private float maxWeaponVolume;

    [Header("Аудио: Движение")]
    public AudioSource movementAudioSource; // Отдельный источник для шагов/прыжков
    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public AudioClip wallJumpSound;
    public float footstepDelay = 0.4f;
    private float nextStepTime = 0f;
    // -----------------------

    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 moveDirection;
    private RaycastHit wallHit;

    void Start()
    {
        // Если забыл прикрепить контроллер вручную
        if (controller == null) controller = GetComponent<CharacterController>();

        // Запоминаем громкость автомата
        if (weaponAudioSource != null)
        {
            maxWeaponVolume = weaponAudioSource.volume;
        }
    }

    void Update()
    {
        // Разделяем логику: даже если weaponAudioSource не назначен, ходить и прыгать персонаж обязан
        if (!isDashing)
        {
            // 1. ЛОГИКА ЗВУКА СТРЕЛЬБЫ
            if (weaponAudioSource != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    weaponAudioSource.Stop();   
                    weaponAudioSource.volume = maxWeaponVolume; 
                    weaponAudioSource.Play();   
                }

                if (Input.GetMouseButton(0))
                {
                    if (!weaponAudioSource.isPlaying) weaponAudioSource.Play();

                    if (weaponAudioSource.time >= (weaponAudioSource.clip.length - timeToCutFromEnd))
                    {
                        weaponAudioSource.time = 0f; 
                    }
                    weaponAudioSource.volume = maxWeaponVolume; 
                }
                else
                {
                    if (weaponAudioSource.isPlaying)
                    {
                        weaponAudioSource.volume = Mathf.MoveTowards(weaponAudioSource.volume, 0f, weaponFadeSpeed * Time.deltaTime);
                        if (weaponAudioSource.volume <= 0f) weaponAudioSource.Stop();
                    }
                }
            }

            // 2. Проверка земли
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Прижимаем к земле
                jumpCount = 0;    // Сбрасываем прыжки
            }

            // 3. Ввод данных
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 inputMove = transform.right * x + transform.forward * z;

            // 4. Расчет движения (земля vs воздух)
            if (isGrounded)
            {
                moveDirection = inputMove * speed;

                // --- ЛОГИКА ШАГОВ ---
                // Если жмем кнопки WASD и пришло время шага
                if (inputMove.magnitude > 0.1f && Time.time >= nextStepTime)
                {
                    PlayMovementSound(footstepSound, 0.4f);
                    nextStepTime = Time.time + footstepDelay;
                }
            }
            else
            {
                // Интерполяция для плавного управления в полете
                moveDirection = Vector3.Lerp(moveDirection, inputMove * speed, airControl * Time.deltaTime);
            }

            controller.Move(moveDirection * Time.deltaTime);

            // 5. Логика прыжков
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded)
                {
                    Jump(); // Обычный прыжок
                    PlayMovementSound(jumpSound, 0.6f);
                }
                else if (TryWallJump())
                {
                    jumpCount = 1; // Позволяем сделать еще один прыжок после стены
                    PlayMovementSound(wallJumpSound, 0.7f);
                }
                else if (jumpCount < maxJumps)
                {
                    Jump(); // Двойной прыжок
                    PlayMovementSound(jumpSound, 0.5f); // Чуть тише для второго прыжка
                }
            }

            // 6. Логика дэша
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(PerformDash(x, z));
            }
        }

        // Применение гравитации (работает всегда, кроме активного дэша)
        if (!isDashing)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpCount++;
    }

    private bool TryWallJump()
    {
        if (CheckDirection(transform.right) || CheckDirection(-transform.right) || CheckDirection(transform.forward))
        {
            Vector3 wallNormal = wallHit.normal;
            moveDirection = wallNormal * wallJumpForce;
            velocity.y = Mathf.Sqrt(wallJumpUpForce * -2f * gravity);
            return true;
        }
        return false;
    }

    private bool CheckDirection(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out wallHit, wallCheckDistance))
        {
            if (wallHit.collider.CompareTag("WallJump"))
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator PerformDash(float x, float z)
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDir = (x == 0 && z == 0) ? transform.forward : (transform.right * x + transform.forward * z).normalized;

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(dashDir * dashSpeed * Time.deltaTime);
            velocity.y = 0; 
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Вспомогательный метод для непересекающихся звуков шагов и прыжков
    private void PlayMovementSound(AudioClip clip, float volume)
    {
        if (movementAudioSource != null && clip != null)
        {
            movementAudioSource.PlayOneShot(clip, volume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, -transform.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, transform.forward * wallCheckDistance);
    }
}