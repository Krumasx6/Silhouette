using UnityEngine;

public class EnemyViewDetection : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D fov;
    [SerializeField] private SpriteRenderer fovRenderer;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Turn red when player enters vision
            fovRenderer.color = new Color(1f, 0f, 0f, 0.8f);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fovRenderer.color = new Color(1f, 1f, 0f, 0.5f);
        }
    }



    
}
