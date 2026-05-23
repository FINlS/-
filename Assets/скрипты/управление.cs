using UnityEngine;
using System.Collections;

public class управление : MonoBehaviour
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
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //
        
        // Блокируем вращение, чтобы персонаж не падал при прыжках
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (isDashing) return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0f, z).normalized;

        // Передаем скорость для базовой анимации бега
        anim.SetFloat("Speed", moveInput.magnitude);

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

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime); //
        
        // Проверка приземления
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void Jump()
    {
        // Запускаем анимацию прыжка
        anim.SetTrigger("Jump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Запускаем анимацию дэша
        anim.SetTrigger("Dash");
        
        // Включаем "неуязвимость" через слой (убедись, что слой создан)
        int oldLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Vector3 dashDirection = moveInput.magnitude > 0 ? moveInput : transform.forward;
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // Возвращаем управление и слой
        gameObject.layer = oldLayer;
        isDashing = false;
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}