using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    private EnemyAI enemyAI;
    
    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSounds"))
        {
            Debug.Log("Enemy heard player sounds.");
            
            // Tell the guard where the sound came from
            enemyAI.OnPlayerSoundDetected(collision.transform.position);
        }
    }
}