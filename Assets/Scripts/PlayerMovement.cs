using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAttributes pa;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttributes>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();

        if (Input.GetButtonDown("Jump") && pa.onGround && !pa.isJumping)
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
        rb.linearVelocity = new Vector2(0f, pa.jumpForce);
        rb.gravityScale = 5f;
        pa.onGround = false;
        pa.isJumping = true;

        float currentVelocity = rb.linearVelocityY;
        yield return new WaitForSeconds(pa.airTime);
        rb.gravityScale = 2f;

        if (rb.linearVelocityX < currentVelocity)
        {
            rb.gravityScale = originalGravity;
        }
    }
}
