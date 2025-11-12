using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private EnemyAttributes ea;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform enemyTransform;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
