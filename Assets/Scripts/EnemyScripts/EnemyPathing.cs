using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private Vector3 currentTarget;
    private float speed = 2f;
    private float reachThreshold = 0.1f;
    private bool movingToB = true;
    void Start()
    {
        currentTarget = pointB.position;
    }
    void Update()
    {   
        MoveEnemy();
    }
    void MoveEnemy()
    {
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, currentTarget, speed * Time.deltaTime);
        if (Vector3.Distance(enemy.transform.position, currentTarget) < reachThreshold)
        {
            if (movingToB)
            {
                currentTarget = pointA.position;
            }
            else
            {
                currentTarget = pointB.position;
            }
            movingToB = !movingToB;
        }
    }


}
