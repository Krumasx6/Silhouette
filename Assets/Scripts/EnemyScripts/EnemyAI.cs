using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float loseTrackTime = 6f;
    [SerializeField] private float stoppingDistance = 0.5f;
    
    private enum EnemyState { Patrolling, Investigating, Chasing }
    private EnemyState currentState = EnemyState.Patrolling;
    
    private EnemyPathing enemyPathing;
    private FieldOfView fieldOfView;
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float patrolWaitCounter = 0f;
    private Vector3 soundInvestigationPoint;
    private float loseTrackCounter = 0f;
    private Transform playerTarget;
    
    private void Start()
    {
        enemyPathing = GetComponent<EnemyPathing>();
        fieldOfView = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
        
        if (patrolPoints.Length > 0)
        {
            Debug.Log("Going to waypoint " + currentPatrolIndex);
            SetTarget(patrolPoints[currentPatrolIndex]);
        }
        else
        {
            Debug.LogError("No patrol points assigned!");
        }
    }
    
    private void Update()
    {
        FaceDirection();
        
        // Always check for player sight first
        if (fieldOfView != null && fieldOfView.playerInSight && fieldOfView.visiblePlayer.Count > 0)
        {
            playerTarget = fieldOfView.visiblePlayer[0];
            SetTarget(playerTarget);
            currentState = EnemyState.Chasing;
            loseTrackCounter = 0f;
            return;
        }
        else
        {
            // Player lost from sight
            if (currentState == EnemyState.Chasing)
            {
                playerTarget = null;
            }
        }
        
        // Handle current state
        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrolling();
                break;
            case EnemyState.Investigating:
                HandleInvestigating();
                break;
            case EnemyState.Chasing:
                HandleChasing();
                break;
        }
    }
    
    private void HandlePatrolling()
    {
        if (patrolPoints.Length == 0)
            return;
        
        float distToPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);
        
        // Check if reached patrol point (use NavMeshAgent's remainingDistance instead)
        if (agent != null && agent.remainingDistance <= stoppingDistance && !agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            
            patrolWaitCounter += Time.deltaTime;
            
            if (patrolWaitCounter >= patrolWaitTime)
            {
                // Move to next patrol point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                patrolWaitCounter = 0f;
                Debug.Log("*** GOING to waypoint " + currentPatrolIndex + " ***");
                SetTarget(patrolPoints[currentPatrolIndex]);
            }
        }
        else
        {
            // Still moving to point
            if (patrolWaitCounter > 0f)
            {
                patrolWaitCounter = 0f;
            }
        }
    }
    
    private void HandleInvestigating()
    {
        float distToSound = Vector3.Distance(transform.position, soundInvestigationPoint);
        
        // Check if reached sound location
        if (distToSound <= stoppingDistance)
        {
            // Reached sound source, wait and listen
            loseTrackCounter += Time.deltaTime;
        }
        else
        {
            // Still moving to sound location
            loseTrackCounter += Time.deltaTime;
        }
        
        // Give up searching
        if (loseTrackCounter >= loseTrackTime)
        {
            currentState = EnemyState.Patrolling;
            patrolWaitCounter = 0f;
            SetTarget(patrolPoints[currentPatrolIndex]);
        }
    }
    
    private void HandleChasing()
    {
        if (playerTarget == null)
        {
            currentState = EnemyState.Patrolling;
            SetTarget(patrolPoints[currentPatrolIndex]);
            return;
        }
        
        // Lose track timer
        loseTrackCounter += Time.deltaTime;
        if (loseTrackCounter >= loseTrackTime)
        {
            playerTarget = null;
            currentState = EnemyState.Patrolling;
            patrolWaitCounter = 0f;
            SetTarget(patrolPoints[currentPatrolIndex]);
        }
    }
    
    // Call this from EnemyListener when sound is detected
    public void OnPlayerSoundDetected(Vector3 soundPosition)
    {
        // Only switch to investigating if not already chasing
        if (currentState != EnemyState.Chasing)
        {
            soundInvestigationPoint = soundPosition;
            currentState = EnemyState.Investigating;
            loseTrackCounter = 0f;
            enemyPathing.GetComponent<EnemyPathing>().SetTargetPosition(soundPosition);
        }
    }
    
    private void SetTarget(Transform target)
    {
        if (target != null && enemyPathing != null)
        {
            enemyPathing.SetTargetTransform(target);
        }
    }
    
    private void FaceDirection()
    {
        if (agent == null || agent.velocity.magnitude < 0.1f)
            return;
        
        Vector3 direction = agent.velocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}