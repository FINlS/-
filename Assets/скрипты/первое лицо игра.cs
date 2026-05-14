using UnityEngine;
using System.Collections;

public class Playerovement : MonoBehaviour 
{
    [Header("Основные настройки")]
    public float moveSpeed = 8f;
    public float jumpForce = 10f;
    [Range(0, 1)] public float airControl = 0.5f; 
    
    [Header("Настройки Дэша")]
    public float dashSpeed = 25f; 
    public float dashDuration = 0.2f; 
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool canAirDash = true; 
    private bool isDashing = false;

    [Header("Бег по стенам (Wall Run)")]
    public LayerMask wallLayer; 
    public float wallRunGravity = 1.2f;
    public float wallJumpForce = 13f; // Немного увеличили силу
    public float wallRunCheckCooldown = 0.25f; // Чуть увеличили паузу
    private float wallRunTimer;
    private bool isWallRunning = false;
    private Vector3 wallNormal;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.sleepThreshold = 0f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (isDashing) return;

        if (wallRunTimer > 0) wallRunTimer -= Time.deltaTime;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveDirection = (transform.right * x + transform.forward * z).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) Jump();
            else if (isWallRunning) WallJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && (isGrounded || canAirDash))
        {
            StartCoroutine(Dash());
        }

        CheckWallRun();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);
        if (isGrounded) canAirDash = true; 

        if (isWallRunning && !isGrounded)
        {
            ApplyWallRunMovement();
        }
        else
        {
            ApplyNormalMovement();
        }
    }

    void ApplyNormalMovement()
    {
        rb.useGravity = true;
        
        // Если мы только что прыгнули от стены, даем импульсу отработать без вмешательства игрока
        float controlModifier = (wallRunTimer > 0) ? 0.1f : 1f;

        Vector3 currentHorizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 targetAirVel = moveDirection * moveSpeed;
        
        Vector3 finalVel = Vector3.Lerp(currentHorizontalVel, targetAirVel, airControl * controlModifier * Time.fixedDeltaTime * 10f);
        rb.linearVelocity = new Vector3(finalVel.x, rb.linearVelocity.y, finalVel.z);
    }

    void CheckWallRun()
    {
        if (isGrounded || wallRunTimer > 0) { isWallRunning = false; return; }

        RaycastHit hit;
        // Лучи стали еще короче (0.6f), чтобы исключить ложные срабатывания
        bool wallRight = Physics.Raycast(transform.position, transform.right, out hit, 0.6f, wallLayer);
        bool wallLeft = Physics.Raycast(transform.position, -transform.right, out hit, 0.6f, wallLayer);

        if ((wallLeft || wallRight) && Input.GetAxisRaw("Vertical") > 0)
        {
            isWallRunning = true;
            canAirDash = true; 
            wallNormal = hit.normal; 
        }
        else
        {
            isWallRunning = false;
        }
    }

    void ApplyWallRunMovement()
    {
        rb.useGravity = false;
        Vector3 wallVel = transform.forward * moveSpeed;
        rb.linearVelocity = new Vector3(wallVel.x, -wallRunGravity, wallVel.z);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void WallJump()
    {
        isWallRunning = false;
        wallRunTimer = wallRunCheckCooldown; 
        
        // ВАЖНО: мы берем нормаль и сильно выталкиваем игрока вбок (2.0f)
        // И умеренно вверх (1.5f). Это гарантирует отлет от стены.
        Vector3 jumpDir = (wallNormal * 2.0f) + (Vector3.up * 1.5f);
        
        // Полный сброс скорости для чистого импульса
        rb.linearVelocity = Vector3.zero; 
        rb.AddForce(jumpDir.normalized * wallJumpForce, ForceMode.Impulse);
    }

    private IEnumerator Dash()
    {
        if (!isGrounded) canAirDash = false; 
        canDash = false;
        isDashing = true;
        rb.useGravity = false;

        Vector3 dashDir = moveDirection.magnitude > 0 ? moveDirection : transform.forward;
        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.useGravity = true;
        isDashing = false;
        rb.linearVelocity *= 0.5f;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}