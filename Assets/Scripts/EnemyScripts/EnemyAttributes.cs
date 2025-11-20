using UnityEngine;

public class EnemyAttributes : MonoBehaviour
{

    // ========== ALERTNESS & DETECTION ==========
    [Header("Alertness & Detection")]
    [Tooltip("Enemy alert")]
    public bool heardAnything = false;
    public bool sawPlayer = false;
    public bool beingCautious = false;
    public bool isChasing = false;
    public bool isInvestigating = false;

    // ========== Patrolling ==========
    [Header("Patrolling")]
    [Tooltip("Enemy is patrolling")]
    public bool isPatrolling = true;
    public int currentPatrolPointIndex = 0;


}
