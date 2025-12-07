using System.Collections;
using UnityEngine;

public class SpawningDeadBodies : MonoBehaviour
{
    [SerializeField] private GameObject deadBodies;
    private Rigidbody2D rb;
    private bool spawnedBodies = false;
    private EnemyAttributes ea;

    void Start()
    {
        ea = GetComponent<EnemyAttributes>();
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator SpawnDeadBody()
    {
        // Prevent multiple spawns
        if (spawnedBodies)
        {
            yield break;
        }

        spawnedBodies = true;
        
        // DISABLE ENEMY IMMEDIATELY
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        
        // Spawn dead body instantly
        Instantiate(deadBodies, ea.gameObject.transform.position, Quaternion.identity);
        
        // Destroy after a short delay
        yield return new WaitForSeconds(0.5f);
        Destroy(ea.gameObject);
    }
}