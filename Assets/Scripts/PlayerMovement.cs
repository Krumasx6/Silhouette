using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAttributes pa;
    // default gravity to use when player is grounded / falling
    private float defaultGravity = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttributes>();
        // ensure the body starts with the desired default gravity
        rb.gravityScale = defaultGravity;
    }

    // Update is called once per frame
    void Update()
    {
        Run();

        if (Input.GetKeyDown(KeyCode.W) && pa.onGround && !pa.isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    private void Run()
    {   
        pa.horizontal = Input.GetAxis("Horizontal");
        Vector2 playerVelocity = rb.linearVelocity;
        playerVelocity.x = pa.horizontal * pa.moveSpeed;
        rb.linearVelocity = playerVelocity;
    }

    private IEnumerator Jump()
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
            rb.gravityScale = defaultGravity; // faster/heavier fall
        }
        else
        {
            // return to the original gravity if still rising or neutral
            rb.gravityScale = originalGravity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pa.onGround = true;
            pa.isJumping = false;
            // restore default gravity on landing
            rb.gravityScale = defaultGravity;
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
