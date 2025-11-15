using UnityEditor;
using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    [SerializeField] private GameObject prefabPlayerSounds;
    private float spawnTimer = 0f;
    private float spawnInterval = 0.5f; // Spawn every 0.5 seconds

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSounds"))
        {
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= spawnInterval)
            {
                Vector3 playerSoundPos = collision.gameObject.transform.position;
                spawnPlayerSoundPrefab(playerSoundPos);
                spawnTimer = 0f;
                Debug.Log("Enemy hearing player sound!");
            }
        }
    }

    void spawnPlayerSoundPrefab(Vector3 position)
    {
        GameObject playerSound = Instantiate(prefabPlayerSounds, position, Quaternion.identity);
    }

}