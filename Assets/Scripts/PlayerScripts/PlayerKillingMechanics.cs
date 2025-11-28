using UnityEngine;

public class PlayerKillingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    private EnemyAttributes currentEnemy;
    
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
                    PerfectlyKill();
                }
                else if (currentEnemy.beingCautious)
                {
                    ChancingKill();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentEnemy = collision.GetComponent<EnemyAttributes>();
            
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

    private void PerfectlyKill()
    {
        Debug.Log("Killed 100%");
        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
        }
        pa.canAttack = false;
        currentEnemy = null;
    }

    private void ChancingKill()
    {
        int randNum = Random.Range(0, 2);
        if (randNum == 1)
        {
            Debug.Log("Killed!!!");
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject);
            }
            pa.canAttack = false;
            currentEnemy = null;
        }
        else
        {
            Debug.Log("Can't kill, the guard is on guard!");
        }
    }
}