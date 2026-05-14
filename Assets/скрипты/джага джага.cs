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

    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 moveDirection;
    private RaycastHit wallHit;

    void Update()
    {
        // Если мы в состоянии дэша, пропускаем обычную логику движения
        if (isDashing) return;

        // 1. Проверка земли
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Прижимаем к земле
            jumpCount = 0;    // Сбрасываем прыжки
        }

        // 2. Ввод данных
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 inputMove = transform.right * x + transform.forward * z;

        // 3. Расчет движения (земля vs воздух)
        if (isGrounded)
        {
            moveDirection = inputMove * speed;
        }
        else
        {
            // Интерполяция для плавного управления в полете
            moveDirection = Vector3.Lerp(moveDirection, inputMove * speed, airControl * Time.deltaTime);
        }

        controller.Move(moveDirection * Time.deltaTime);

        // 4. Логика прыжков
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump(); // Обычный прыжок
            }
            else if (TryWallJump())
            {
                // Прыжок от стены (DoWallJump уже вызывается внутри TryWallJump)
                jumpCount = 1; // Позволяем сделать еще один прыжок в воздухе после стены
            }
            else if (jumpCount < maxJumps)
            {
                Jump(); // Двойной прыжок
            }
        }

        // 5. Логика дэша
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(PerformDash(x, z));
        }

        // 6. Применение гравитации
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpCount++;
    }

    private bool TryWallJump()
    {
        // Проверяем направления: вправо, влево, вперед
        if (CheckDirection(transform.right) || CheckDirection(-transform.right) || CheckDirection(transform.forward))
        {
            Vector3 wallNormal = wallHit.normal;
            
            // Отталкиваемся от нормали стены + придаем импульс вверх
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
            // Проверяем, есть ли у стены нужный тег
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

        // Направление дэша: куда жмем, туда и летим. Если не жмем ничего — летим вперед.
        Vector3 dashDir = (x == 0 && z == 0) ? transform.forward : (transform.right * x + transform.forward * z).normalized;

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(dashDir * dashSpeed * Time.deltaTime);
            velocity.y = 0; // "Замораживаем" падение в момент рывка
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Отрисовка лучей в редакторе для отладки WallJump
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, -transform.right * wallCheckDistance);
        Gizmos.DrawRay(transform.position, transform.forward * wallCheckDistance);
    }
}