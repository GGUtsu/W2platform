using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 12f;
    public float runJumpMultiplier = 1.25f;
    private float currentSpeed;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Physics")]
    public float jumpGravityMultiplier = 2.2f;

    [Header("Coyote Time (Jump leniency)")]
    public float coyoteTime = 0.17f;
    private float coyoteTimeCounter = 0f;

    [Header("Double Jump")]
    public int maxJumpCount = 2;
    private int jumpCount = 0;

    [Header("Sound")]
    public AudioClip jumpSound;
    [Range(0f, 1f)] public float jumpSoundVolume = 0.7f;
    private AudioSource audioSource;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.7f;
    private bool isDashing = false;
    private float dashTime = 0f;
    private float lastDashTime = -10f;

    // --- Health System ---
    [Header("Player Health")]
    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    public bool facingRight = true; // <-- เปลี่ยนจาก private เป็น public
    private bool canMove = true;

    private float cachedGravityScale = 1f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isCurrentlyRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) cachedGravityScale = rb.gravityScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        animator = GetComponent<Animator>();

        // Initialize health
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!canMove)
        {
            SetIdleAnimation();
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isDashing && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }

        if (isDashing) {
            UpdateAnimationState();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        HandleMovementLogic();

        isCurrentlyRunning = Input.GetKey(KeyCode.LeftShift) && moveInput != 0 && currentSpeed == runSpeed;

        // --- Improved Jump: รองรับแรงโน้มถ่วงสลับโลก ---
        bool jumpPressed = Input.GetButtonDown("Jump");
        if (jumpPressed)
        {
            if ((coyoteTimeCounter > 0f && jumpCount < 1) ||
                (!isGrounded && jumpCount < maxJumpCount))
            {
                // คำนวณ gravity absolute เพื่อ Sqrt ไม่ติดลบ
                float gravityAbs = Mathf.Abs(Physics2D.gravity.y * cachedGravityScale * jumpGravityMultiplier);
                float targetHeight = jumpForce;

                if (isCurrentlyRunning) targetHeight *= runJumpMultiplier;

                float newVy = Mathf.Sqrt(2f * gravityAbs * targetHeight);
                // ใส่ทิศทาง force ตามโลกปัจจุบัน
                newVy *= Mathf.Sign(cachedGravityScale);

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, newVy);

                if (jumpSound != null && audioSource != null)
                    audioSource.PlayOneShot(jumpSound, jumpSoundVolume);

                if (animator != null)
                    animator.SetTrigger("Jump");

                if (coyoteTimeCounter > 0f) coyoteTimeCounter = 0f;
                jumpCount++;
            }
        }
        // -------------------------------------------------

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        UpdateAnimationState();

        // เพิ่มแรงถ่วง/ความลื่นของกระโดด รองรับโลกคว่ำ
        if (rb != null && !isGrounded)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (jumpGravityMultiplier - 1f) * Time.deltaTime * cachedGravityScale;
        }
    }

    void UpdateAnimationState()
    {
        if (animator == null) return;

        // ปรับค่า velocity.y ตามโลกปัจจุบัน เพื่อให้อะนิเมชัน Falling/Jumping ถูกรองรับทุกโลก
        float adjustedVelocityY = rb.linearVelocity.y * Mathf.Sign(cachedGravityScale);

        bool isFalling = adjustedVelocityY < -0.1f && !isGrounded;
        bool isJumping = adjustedVelocityY > 0.1f && !isGrounded;
        bool isMovingOnGround = Mathf.Abs(moveInput) > 0.01f && isGrounded;
        bool isIdleOnGround = Mathf.Abs(moveInput) <= 0.01f && isGrounded;

        // Dash
        if (isDashing)
        {
            animator.SetBool("IsDashing", true);
            animator.SetBool("IsMove", false);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            return;
        }
        else animator.SetBool("IsDashing", false);

        animator.SetBool("IsMove", isMovingOnGround || isIdleOnGround);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
    }

    void SetIdleAnimation()
    {
        if (animator == null) return;
        animator.SetBool("IsMove", true);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsDashing", false);
    }

    void FixedUpdate()
    {
        if (canMove && !isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
    }

    void HandleMovementLogic()
    {
        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && moveInput != 0;
        currentSpeed = isTryingToRun ? runSpeed : walkSpeed;
    }

    void Flip()
    {
        facingRight = !facingRight;

        // ยกเลิกการใช้ spriteRenderer.flipX แล้วเปลี่ยนมาใช้การกลับ Scale แทน
        // เพื่อให้ FirePoint และ Object ลูกตัวอื่นๆ พลิกตามตัวละครไปด้วย
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        float direction = facingRight ? 1f : -1f;

        if (animator != null)
            animator.SetBool("IsDashing", true);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;

        if (animator != null)
            animator.SetBool("IsDashing", false);
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
        if (!state) rb.linearVelocity = Vector2.zero;
        if (!state) SetIdleAnimation();
    }

    // ฟังก์ชันสำหรับนำมาเรียกเมื่อโดนดาเมจ
    public void TakeDamage(int damage)
    {
        // ถ้า Player ตายไปแล้วก็ไม่ต้องทำอะไร
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // ตายทันทีและไม่ให้เกิดใหม่ (ลบ GameObject นี้ออก)
            Destroy(this.gameObject);
        }
        // คุณอาจจะเพิ่มเอฟเฟค Hurt Animation หรือเสียงที่นี่ได้ตามต้องการ
    }

    // --- ฟังก์ชันสลับโลก/แรงโน้มถ่วง (Toggle gravity direction) ---
    public void ToggleGravity()
    {
        cachedGravityScale *= -1f;
        rb.gravityScale = cachedGravityScale;

        // Flip player upside down visually (รวม groundCheck ติดไปด้วย)
        Vector3 scaler = transform.localScale;
        scaler.y *= -1;
        transform.localScale = scaler;

        // Reset Y velocity to avoid overshooting/falling through ground
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }
}