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
        spawnedBodies = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
        Instantiate(deadBodies, ea.gameObject.transform.position, Quaternion.identity);
        ea.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Destroy(ea.gameObject);
    }
}
