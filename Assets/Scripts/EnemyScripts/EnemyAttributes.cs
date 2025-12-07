using UnityEngine;

public class EnemyAttributes : MonoBehaviour
{

    // ========== ALERTNESS & DETECTION ==========
    [Header("Alertness & Detection")]
    [Tooltip("Enemy alert")]
    public bool heardAnything = false;
    public bool sawPlayer = false;
    public bool isCautious = false;
    public bool isAlert = false;
    public bool unaware = true;
    public bool isInvestigating = false;
    public bool isChasing = false;

    // ========== Patrolling ==========
    [Header("Patrolling")]
    [Tooltip("Enemy is patrolling")]
    public bool isPatrolling = true;
    public int currentPatrolPointIndex = 0;


    // ========== INTERNAL STATE ==========
    [Header("Internal State")]
    [Tooltip("Checking if the enemy is dead")]
    public bool isDead = false;

}
