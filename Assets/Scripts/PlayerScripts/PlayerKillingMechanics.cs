using System.Collections;
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
                    StartCoroutine(Kill());
                }
                else if (currentEnemy.beingCautious)
                {
                    StartCoroutine(AttemptKill());
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
    
    private IEnumerator Kill()
    {
        if (currentEnemy != null)
        {
            currentEnemy.isDead = true;
            yield return new WaitForSeconds(0.5f);
            Destroy(currentEnemy.gameObject);
        }
        pa.canAttack = false;
        currentEnemy = null;
    }

    private IEnumerator AttemptKill()
    {
        int randNum = Random.Range(0, 2);
        if (randNum == 1)
        {
            currentEnemy.isDead = true;
            yield return new WaitForSeconds(0.5f);
            Destroy(currentEnemy.gameObject);
        }
        else
        {
            //not kill
        }
    }
}