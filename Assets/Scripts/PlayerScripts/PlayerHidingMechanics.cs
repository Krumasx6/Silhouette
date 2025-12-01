using UnityEngine;

public class PlayerHidingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Debounce Settings")]
    [SerializeField] private float hideDebounceTime = 2f;
    [SerializeField] private float unhideDebounceTime = 2f;
    
    private Transform hideSpot;
    private Vector3 playerCurrentPosition;
    private float lastActionTime = -999f;
    private bool canPerformAction = true;
    
    void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && canPerformAction)
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
        
        canPerformAction = false;
        lastActionTime = Time.time;
        Invoke(nameof(EnableAction), hideDebounceTime);
        
        Debug.Log("Hidden! Wait " + hideDebounceTime + " seconds to unhide");
    }

    private void Unhide()
    {
        transform.parent.position = playerCurrentPosition;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        spriteRenderer.enabled = true;
        
        pa.isHiding = false;
        pa.canHide = false;
        hideSpot = null;
        
        canPerformAction = false;
        lastActionTime = Time.time;
        Invoke(nameof(EnableAction), unhideDebounceTime);
        
        Debug.Log("Unhidden! Wait " + unhideDebounceTime + " seconds to hide again");
    }
    
    private void EnableAction()
    {
        canPerformAction = true;
    }
}