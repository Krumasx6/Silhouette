using System.Collections;
using UnityEngine;

public class PlayerKillingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    private EnemyAttributes currentEnemy;
    private SpawningDeadBodies spawnPrefabs;

    void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
    }

    void Update()
    {
        if (currentEnemy != null)
        {
            if (currentEnemy.isChasing)
            {
                pa.canAttack = false;
                currentEnemy = null;
            }
        }

        if (pa.canAttack && Input.GetKeyDown(KeyCode.E))
        {
            if (currentEnemy != null)
            {
                if (!currentEnemy.isChasing && !currentEnemy.beingCautious)
                {
                    Kill();
                }
                else if (currentEnemy.beingCautious)
                {
                    AttemptKill();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentEnemy = collision.GetComponent<EnemyAttributes>();
            spawnPrefabs = collision.GetComponent<SpawningDeadBodies>();
            
            if (currentEnemy != null)
            {
                if (!currentEnemy.isChasing)
                {   
                    pa.canAttack = true;
                    Debug.Log("Press E to attack!");
                }
                else
                {
                    pa.canAttack = false;
                    Debug.Log("Enemy is chasing - can't attack!");
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            pa.canAttack = false;
            currentEnemy = null;
            Debug.Log("Left attack range");
        }
    }
    
    private void Kill()
    {
        if (currentEnemy != null)
        {
            currentEnemy.isDead = true;
            spawnPrefabs.StartCoroutine(spawnPrefabs.SpawnDeadBody());
        }
        pa.canAttack = false;
        currentEnemy = null;
    }

    private void AttemptKill()
    {
        int randNum = Random.Range(0, 2);
        if (randNum == 1)
        {
            spawnPrefabs.StartCoroutine(spawnPrefabs.SpawnDeadBody());
        }
        else
        {
            //not kill
        }
    }
}