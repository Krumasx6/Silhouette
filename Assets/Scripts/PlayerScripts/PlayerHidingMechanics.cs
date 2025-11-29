using UnityEngine;

public class PlayerHidingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    private Transform hideSpot;
    private Vector3 originalPosition;
    
    void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
        rb = GetComponentInParent<Rigidbody2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (pa.canHide && !pa.isHiding)
            {
                Hide();
            }
            else if (pa.isHiding)
            {
                Unhide();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HideSpot"))
        {
            hideSpot = collision.transform;
            pa.canHide = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HideSpot"))
        {
            hideSpot = null;
            pa.canHide = false;
            
            if (pa.isHiding)
            {
                Unhide();
            }
        }
    }

    private void Hide()
    {
        if (hideSpot == null)
        {
            return;
        }
        
        if (transform.parent == null)
        {
            return;
        }
        
        originalPosition = transform.parent.position;
        
        rb.linearVelocity = Vector2.zero;
        transform.parent.position = hideSpot.position;
        
        spriteRenderer.enabled = false;
        rb.simulated = false;
        
        pa.isHiding = true;
        Debug.Log("Hidden!");
    }

    private void Unhide()
    {
        transform.parent.position = originalPosition;
        
        spriteRenderer.enabled = true;
        rb.simulated = true;
        
        pa.isHiding = false;
        Debug.Log("Unhidden!");
    }
}