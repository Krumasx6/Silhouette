using UnityEngine;

public class PlayerHidingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Transform hideSpot;
    private Vector3 playerCurrentPosition;
    
    void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (pa.canHide && !pa.isHiding && hideSpot != null)
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
            Debug.Log("Press R to hide");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HideSpot") && !pa.isHiding)
        {
            hideSpot = null;
            pa.canHide = false;
        }
    }

    private void Hide()
    {
        playerCurrentPosition = transform.parent.position;
        
        transform.parent.position = hideSpot.position;
        
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        
        spriteRenderer.enabled = false;
        
        pa.isHiding = true;
        Debug.Log("Hidden!");
    }

    private void Unhide()
    {
        transform.parent.position = playerCurrentPosition;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        spriteRenderer.enabled = true;
        
        pa.isHiding = false;
        pa.canHide = false;
        hideSpot = null;
        Debug.Log("Unhidden!");
    }
}