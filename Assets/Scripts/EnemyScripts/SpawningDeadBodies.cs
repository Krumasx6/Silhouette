using UnityEngine;

public class SpawningDeadBodies : MonoBehaviour
{
    [SerializeField] private GameObject deadBodies;
    private bool spawnedBodies = false;

    private EnemyAttributes ea;

    void Start()
    {
        ea = GetComponent<EnemyAttributes>();
    }

    void Update()
    {
        if (ea.isDead && !spawnedBodies)
        {   
            SpawnDeadBody();
        }
    }

    private void SpawnDeadBody()
    {
        spawnedBodies = true;
        Debug.Log("Spawned dead bodies");
    }
}
