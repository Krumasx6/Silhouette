//PlayerMovement
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAttributes pa;
    public BoxCollider2D boxCol;

    Vector2 standingSize;
    Vector2 crouchSize = new Vector2(1f, 0.5f);
    Vector2 standingOffset;
    Vector2 crouchOffset = new Vector2(0f, -0.25f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttributes>();
        boxCol = GetComponent<BoxCollider2D>();
        
        rb.gravityScale = pa.defaultGravity;
        standingSize = boxCol.size;
        standingOffset = boxCol.offset;
    }

    void Update()
    {   
        if (pa.isVaulting)
            return;
        pa.horizontal = Input.GetAxis("Horizontal");
        pa.isRunning = Input.GetKey(KeyCode.LeftShift) && pa.onGround && Mathf.Abs(pa.horizontal) > 0.01f;

        if (Input.GetKey(KeyCode.W) && pa.onGround && !pa.isJumping)
        {
            StartCoroutine(Jump());
        }   


        if (Input.GetKeyDown(KeyCode.LeftControl) && pa.onGround && !pa.isSliding && !pa.isJumping)
        {
            pa.isCrouching = !pa.isCrouching;
        }

        if (pa.isJumping || pa.isSliding)
            pa.isCrouching = false;

        if (!pa.isSliding && Input.GetKeyDown(KeyCode.S) && pa.onGround && !pa.isJumping)
        {
            float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
            float speedThreshold = pa.runSpeed * pa.slideSpeedThreshold;

            if (currentSpeed >= speedThreshold && pa.isRunning && Time.time - pa.lastSlideTime > pa.slideCooldown)
            {
                StartCoroutine(Slide());
                pa.lastSlideTime = Time.time;
            }
        }

        if (pa.isCrouching)
            pa.isRunning = false;
            
        pa.canVault = pa.isInVaultZone && pa.isRunning && pa.onGround && !pa.isVaulting;

        if (pa.canVault && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(VaultRoutine());
        }

    }

    void FixedUpdate()
    {
        if (pa.isClimbing)
        {
            boxCol.size = standingSize;
            boxCol.offset = standingOffset;
            return;
        }

        if (!pa.isSliding)
            Run();

        Flip();

        if ((pa.isCrouching || pa.isSliding) && !pa.isJumping)
        {
            boxCol.size = crouchSize;
            boxCol.offset = crouchOffset;
        }
        else
        {
            boxCol.size = standingSize;
            boxCol.offset = standingOffset;
        }
    }

    private IEnumerator VaultRoutine()
    {
        pa.canVault = false;
        pa.isVaulting = true;

        // Lock facing direction
        bool facingRight = pa.isFacingRight;

        // Store original collider
        Vector2 originalSize = boxCol.size;
        Vector2 originalOffset = boxCol.offset;

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        float teleportDistance = 2f;
        Vector2 newPosition = transform.position;

        // Optional pre-lift delay
        yield return new WaitForSeconds(0.1f);

        // Shrink collider
        boxCol.size = crouchSize;
        boxCol.offset = crouchOffset;

        // Lift + horizontal move
        newPosition.y += 0.5f;
        newPosition.x += facingRight ? teleportDistance : -teleportDistance;
        transform.position = newPosition;

        // Optional pause
        yield return new WaitForSeconds(0.3f);

        // Restore collider
        boxCol.size = originalSize;
        boxCol.offset = originalOffset;

        pa.isVaulting = false;

        // Cooldown
        yield return new WaitForSeconds(0.5f);
        pa.canVault = true;
    }



    private void Run()
    {
        float baseSpeed = pa.isCrouching ? pa.crouchSpeed : (pa.isRunning ? pa.runSpeed : pa.walkSpeed);
        float targetSpeed = pa.horizontal * baseSpeed;
        bool hasInput = Mathf.Abs(pa.horizontal) > 0.01f;
        
        float accel = pa.onGround ? 
            (hasInput ? pa.groundAcceleration : pa.groundDeceleration) : 
            (hasInput ? pa.airAcceleration : pa.airDeceleration);

        Vector2 velocity = rb.linearVelocity;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;
    }

    private IEnumerator Jump()
    {
        if (!pa.isCrouching && !pa.isSliding)
        {
            float originalGravity = rb.gravityScale;
            Vector2 lv = rb.linearVelocity;
            lv.y = pa.jumpForce;
            rb.linearVelocity = lv;

            rb.gravityScale = 2f;
            pa.onGround = false;
            pa.isJumping = true;

            yield return new WaitForSeconds(pa.airTime);

            if (!pa.isClimbing)
            {
                rb.gravityScale = rb.linearVelocity.y < 0f ? pa.defaultGravity : originalGravity;
            }
        }
    }

    private IEnumerator Slide()
    {
        pa.isSliding = true;
        pa.isCrouching = true;

        float slideDirection = Mathf.Sign(rb.linearVelocity.x);
        if (slideDirection == 0f) slideDirection = 1f;

        rb.linearVelocity = new Vector2(slideDirection * pa.slideSpeed, rb.linearVelocity.y);
        boxCol.size = crouchSize;
        boxCol.offset = crouchOffset;

        float timer = 0f;
        while (timer < pa.slideDuration && Mathf.Abs(rb.linearVelocity.x) > 1f)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x *= pa.slideFriction;
            rb.linearVelocity = velocity;

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        pa.isSliding = false;
        if (!Input.GetKey(KeyCode.S))
        {
            pa.isCrouching = false;
            boxCol.size = standingSize;
            boxCol.offset = standingOffset;
        }
    }

    // Flipping Sprites
    private void Flip()
    {
        
        if (pa.horizontal > 0)
        {
            transform.localScale = new Vector2(1f, 1f);
            pa.isFacingRight = true;
        }
        else if (pa.horizontal < 0)
        {
            transform.localScale = new Vector2(-1f, 1f);
            pa.isFacingRight = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground") && !collision.gameObject.CompareTag("Vaultable"))
            return;

        if (pa.isClimbing)
            return;

        pa.onGround = true;

        if (rb.linearVelocity.y <= 0f)
        {
            pa.isJumping = false;
            rb.gravityScale = pa.defaultGravity;
        }
    }


}