using UnityEngine;
using System.Collections;

public class управление : Sounds
{
    [Header("Основные настройки")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    
    [Header("Настройки Дэша")]
    public float dashSpeed = 300f;
    public float dashDuration = 1f;
    public float dashCooldown = 2f;
    private bool canDash = true;
    private bool isDashing = false;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Animator anim;
    private bool isGrounded;

    [Header("Настройки звука стрельбы")]
    public AudioSource weaponAudioSource;
    public float fadeSpeed = 5f;        
    private float maxVolume;            
    public float timeToCutFromEnd = 0.3f;

    [Header("Настройки звука шагов")]
    public float footstepDelay = 0.4f;   // Как часто будут звучать шаги (чем меньше, тем быстрее)
    private float nextStepTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); 
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (weaponAudioSource != null)
        {
            maxVolume = weaponAudioSource.volume;
        }
    }

    void Update()
    {
        // Убрали жесткий return, чтобы если weaponAudioSource временно равен null, 
        // персонаж всё равно мог ходить, прыгать и воспроизводить шаги.
        
        if (!isDashing)
        {
            // --- ЛОГИКА СТРЕЛЬБЫ ---
            if (weaponAudioSource != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    weaponAudioSource.Stop();   
                    weaponAudioSource.volume = maxVolume; 
                    weaponAudioSource.Play();   
                }

                if (Input.GetMouseButton(0))
                {
                    if (!weaponAudioSource.isPlaying)
                    {
                        weaponAudioSource.Play();
                    }

                    if (weaponAudioSource.time >= (weaponAudioSource.clip.length - timeToCutFromEnd))
                    {
                        weaponAudioSource.time = 0f; 
                    }

                    weaponAudioSource.volume = maxVolume; 
                }
                else
                {
                    if (weaponAudioSource.isPlaying)
                    {
                        weaponAudioSource.volume = Mathf.MoveTowards(weaponAudioSource.volume, 0f, fadeSpeed * Time.deltaTime);

                        if (weaponAudioSource.volume <= 0f)
                        {
                            weaponAudioSource.Stop();
                        }
                    }
                }
            }

            // --- ЛОГИКА ДВИЖЕНИЯ ---
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            moveInput = new Vector3(x, 0f, z).normalized;

            anim.SetFloat("Speed", moveInput.magnitude);

            // --- ЛОГИКА ШАГОВ ---
            // Проверяем: персонаж жмет кнопки движения, он на земле и пришло время для шага
            if (moveInput.magnitude > 0 && isGrounded && Time.time >= nextStepTime)
            {
                // Проверяем, добавил ли ты второй элемент в массив sounds в Инспекторе
                if (sounds != null && sounds.Length > 1 && sounds[1] != null)
                {
                    // Играем sound[1] (звук шага) через встроенную систему Sounds
                    PlaySound(sounds[1], 0.3f, false, 0.9f, 1.1f); 
                    
                    nextStepTime = Time.time + footstepDelay;
                }
            }

            // Прыжок
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }

            // Дэш
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime); 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void Jump()
    {
        anim.SetTrigger("Jump");
        if (sounds != null && sounds.Length > 0)
        {
            PlaySound(sounds[0], 0.2f); // Звук прыжка (элемент 0)
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash");
        
        int oldLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Vector3 dashDirection = moveInput.magnitude > 0 ? moveInput : transform.forward;
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        gameObject.layer = oldLayer;
        isDashing = false;
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}