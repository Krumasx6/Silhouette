using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    private EnemyAI enemyAI;
    private EnemyAttributes ea;
    
    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
        ea = GetComponentInParent<EnemyAttributes>();
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSounds"))
        {
            ea.heardAnything = true;
            // Tell the guard where the sound came from
            enemyAI.OnPlayerSoundDetected(collision.transform.position);
        }
        else
        {
            ea.heardAnything = false;
        }
    }
}