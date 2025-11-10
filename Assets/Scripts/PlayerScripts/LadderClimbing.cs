//LadderClimbing
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAttributes))]
[RequireComponent(typeof(BoxCollider2D))]
public class LadderClimbing : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAttributes pa;
    private BoxCollider2D boxCol;

    [SerializeField] private float climbDownSpeedMultiplier = 2f; // Climb down is 2x faster

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttributes>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!pa.isLadder)
            return;

        pa.vertical = Input.GetAxis("Vertical");

        // Only engage ladder when player presses W or S
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            if (!pa.isClimbing)
            {
                pa.isClimbing = true;
                pa.isJumping = false;
                pa.isSliding = false;
                pa.isCrouching = false;
                pa.horizontal = 0f;
                rb.gravityScale = 0f;  // Set gravity to 0 ONLY when starting to climb
                rb.linearVelocity = Vector2.zero;
                
                // Snap to ladder X
                transform.position = new Vector2(pa.ladderX, transform.position.y + 0.1f);
            }
        }
    }

    void FixedUpdate()
    {
        if (pa.isClimbing)
        {
            bool isClimbingDown = pa.vertical < 0; // Pressing S (down)
            
            if (isClimbingDown)
            {   
                transform.position = new Vector2(pa.ladderX, transform.position.y);
                // Climbing down: no horizontal movement, faster speed
                rb.linearVelocity = new Vector2(0f, pa.vertical * pa.climbSpeed * climbDownSpeedMultiplier);
            }
            else
            {
                // Climbing up or idle: can move horizontally
                rb.linearVelocity = new Vector2(pa.horizontal * pa.climbSpeed, pa.vertical * pa.climbSpeed);
            }
            
            pa.isJumping = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            pa.isLadder = true;
            pa.ladderX = collision.bounds.center.x;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            pa.isLadder = false;
            pa.isClimbing = false;
            rb.gravityScale = pa.defaultGravity;
        }
    }
}