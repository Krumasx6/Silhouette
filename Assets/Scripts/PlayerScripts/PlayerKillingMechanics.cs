using UnityEngine;

public class PlayerKillingMechanics : MonoBehaviour
{
    private PlayerAttributes pa;
    void Start()
    {
        pa = GetComponent<PlayerAttributes>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAttributes ea = collision.gameObject.GetComponent<EnemyAttributes>();

            if (!ea.isChasing && !ea.beingCautious)
            {
                pa.canAttack = true;
                // 100% chance to kill
            } 
            else if (ea.beingCautious)
            {
                pa.canAttack = true;
                // 50% chance to kill
            }
            else
            {
                pa.canAttack = false;
                // 0% chance to kill
            }
        }
    }

}
