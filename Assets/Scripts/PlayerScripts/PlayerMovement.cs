using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAttributes pa;

    public BoxCollider2D boxCol;
    public Animation anim;

    Vector2 standingSize;
    Vector2 crouchSize = new Vector2(1f, 0.5f);
    Vector2 standingOffset;
    Vector2 crouchOffset = new Vector2(0f, -0.25f);

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttributes>();
        // ensure the body starts with the desired default gravity
        rb.gravityScale = pa.defaultGravity;
        // if PlayerAttributes provides a moveSpeed, prefer it
        if (pa != null && pa.moveSpeed <= 0f)
            pa.moveSpeed = pa.defaultMoveSpeed; // assign default if attribute not set

        standingSize = boxCol.size;
        standingOffset = boxCol.offset;
    }

    // Update is called once per frame - read input here
    void Update()
    {
        // read horizontal input every frame for snappy control
        pa.horizontal = Input.GetAxis("Horizontal");

        // hold Shift to run (only when on ground)
        pa.isRunning = (Input.GetKey(KeyCode.LeftShift)) && pa.onGround && Mathf.Abs(pa.horizontal) > 0.01f;

        if (Input.GetKeyDown(KeyCode.W) && pa.onGround && !pa.isJumping)
        {
            StartCoroutine(Jump());
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl) && pa.onGround && !pa.isJumping)
        {
            if (Time.time - pa.lastCrouchToggleTime > pa.crouchToggleCooldown)
            {
                pa.isCrouching = !pa.isCrouching;
                pa.lastCrouchToggleTime = Time.time;
            }
        } 
    }

    // FixedUpdate is used for physics-driven movement
    void FixedUpdate()
    {
        Run();

        if (pa.isCrouching && !pa.isJumping)
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

    private void Run()
    {
        // target horizontal speed based on input
        float baseSpeed = pa.isRunning ? pa.runSpeed : pa.walkSpeed;
        float targetSpeed = pa.horizontal * baseSpeed;

        // choose acceleration/deceleration depending on grounded state
        bool hasInput = Mathf.Abs(pa.horizontal) > 0.01f;
        float accel = pa.onGround ? (hasInput ? pa.groundAcceleration : pa.groundDeceleration) : (hasInput ? pa.airAcceleration : pa.airDeceleration);

        // move current velocity toward target using MoveTowards for consistent control
        Vector2 lv = rb.linearVelocity;
        float maxDelta = accel * Time.fixedDeltaTime;
        lv.x = Mathf.MoveTowards(lv.x, targetSpeed, maxDelta);
        rb.linearVelocity = lv;
    }

    private IEnumerator Jump()
    {
        if (!pa.isCrouching)
        {
            float originalGravity = rb.gravityScale;
            // set upward velocity for the jump (preserve horizontal velocity)
            Vector2 lv = rb.linearVelocity;
            lv.y = pa.jumpForce;
            rb.linearVelocity = lv;
            // use a lighter gravity while rising so the jump feels floaty
            rb.gravityScale = 2f;
            pa.onGround = false;
            pa.isJumping = true;

            // wait for a short air-time window (tunable in PlayerAttributes)
            yield return new WaitForSeconds(pa.airTime);

            // if the player is falling (negative Y velocity) make the fall heavier
            if (rb.linearVelocity.y < 0f)
            {
                rb.gravityScale = pa.defaultGravity; // faster/heavier fall
            }
            else
            {
                // return to the original gravity if still rising or neutral
                rb.gravityScale = originalGravity;
            }
        }
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pa.onGround = true;
            pa.isJumping = false;
            // restore default gravity on landing
            rb.gravityScale = pa.defaultGravity;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pa.onGround = false;
        }
    }
}
