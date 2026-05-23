using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("Components")]
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Movement")]
    public float speed = 11f;
    public float groundAcceleration = 25f;
    public float groundDeceleration = 30f;
    public float airAcceleration = 12f;

    [Header("Gravity")]
    public float gravity = -42f;
    public float jumpHeight = 2.2f;

    [Header("Jump")]
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashSpeed = 30f;
    public float dashTime = 0.16f;
    public float dashCooldown = 1f;

    [Header("Wall")]
    public float wallCheckDistance = 0.8f;
    public float wallSlideSpeed = -2.5f;
    public float wallJumpUpForce = 9f;
    public float wallJumpPushForce = 12f;

    [Header("Ground")]
    public float groundRadius = 0.35f;

    private Vector3 moveDir;
    private Vector3 velocity;

    private bool grounded;
    private bool dashing;
    private bool canDash = true;

    private int jumps;

    private RaycastHit wallHit;
    private Vector3 wallNormal;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundCheck();

        if (!dashing)
        {
            Movement();
            WallSlide();
            JumpInput();
            DashInput();
        }

        Gravity();
        MoveFinal();
    }

    // -------------------------
    // GROUND
    // -------------------------
    void GroundCheck()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumps = 0;
        }
    }

    // -------------------------
    // MOVEMENT (LESS “PLOW” FEEL)
    // -------------------------
    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = (transform.right * x + transform.forward * z).normalized;
        Vector3 target = input * speed;

        float accel = grounded
            ? (input.magnitude > 0 ? groundAcceleration : groundDeceleration)
            : airAcceleration;

        moveDir = Vector3.MoveTowards(moveDir, target, accel * Time.deltaTime);
    }

    // -------------------------
    // JUMP
    // -------------------------
    void JumpInput()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (grounded)
        {
            Jump();
            return;
        }

        if (TryWallJump()) return;

        if (jumps < maxJumps)
        {
            Jump();
        }
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumps++;
    }

    // -------------------------
    // WALL SLIDE (SMOOTH BUT NOT STICKY)
    // -------------------------
    void WallSlide()
    {
        if (grounded) return;

        if (IsWall() && velocity.y < 0)
        {
            velocity.y = Mathf.MoveTowards(
                velocity.y,
                wallSlideSpeed,
                25f * Time.deltaTime
            );
        }
    }

    // -------------------------
    // WALL JUMP
    // -------------------------
    bool TryWallJump()
    {
        if (!IsWall()) return false;

        moveDir = wallNormal * wallJumpPushForce;
        velocity.y = Mathf.Sqrt(wallJumpUpForce * -2f * gravity);

        return true;
    }

    bool IsWall()
    {
        Vector3 origin = transform.position + Vector3.up;

        if (Physics.Raycast(origin, transform.right, out wallHit, wallCheckDistance) ||
            Physics.Raycast(origin, -transform.right, out wallHit, wallCheckDistance) ||
            Physics.Raycast(origin, transform.forward, out wallHit, wallCheckDistance) ||
            Physics.Raycast(origin, -transform.forward, out wallHit, wallCheckDistance))
        {
            if (wallHit.collider.CompareTag("WallJump"))
            {
                wallNormal = wallHit.normal;
                return true;
            }
        }

        return false;
    }

    // -------------------------
    // DASH (LESS BUGGY)
    // -------------------------
    void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        dashing = true;

        Vector3 dir = moveDir.magnitude > 0.1f ? moveDir.normalized : transform.forward;

        float t = 0;

        while (t < dashTime)
        {
            controller.Move(dir * dashSpeed * Time.deltaTime);
            velocity.y = 0;
            t += Time.deltaTime;
            yield return null;
        }

        dashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // -------------------------
    // GRAVITY
    // -------------------------
    void Gravity()
    {
        if (!grounded || velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    // -------------------------
    // FINAL MOVE (IMPORTANT FIX)
    // -------------------------
    void MoveFinal()
    {
        Vector3 final = moveDir;
        final.y = velocity.y;

        controller.Move(final * Time.deltaTime);
    }
}