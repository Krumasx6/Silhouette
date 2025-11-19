using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    
    // Sight settings
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float sightAngle = 60f;
    [SerializeField] private Transform sightOrigin;
    
    // Chase settings
    [SerializeField] private float loseTrackTime = 6f;
    
    private enum GuardState { Patrolling, Investigating, Chasing }
    private GuardState currentState = GuardState.Patrolling;
    
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float patrolWaitCounter = 0f;
    private Vector3 soundInvestigationPoint;
    private float loseTrackCounter = 0f;
    private Transform playerTarget;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        if (patrolPoints.Length > 0)
            GoToNextPatrolPoint();
    }
    
    private void Update()
    {
        // Always check for player sight first (highest priority)
        if (CanSeePlayer(out Transform player))
        {
            playerTarget = player;
            currentState = GuardState.Chasing;
            loseTrackCounter = 0f;
            return;
        }
        
        // Handle current state
        switch (currentState)
        {
            case GuardState.Patrolling:
                HandlePatrolling();
                break;
            case GuardState.Investigating:
                HandleInvestigating();
                break;
            case GuardState.Chasing:
                HandleChasing();
                break;
        }
    }
    
    private void HandlePatrolling()
    {
        if (patrolPoints.Length == 0)
            return;
        
        Transform currentPoint = patrolPoints[currentPatrolIndex];
        float distToPoint = Vector3.Distance(transform.position, currentPoint.position);
        
        if (distToPoint > 0.5f)
        {
            // Move toward patrol point
            agent.SetDestination(currentPoint.position);
        }
        else
        {
            // Reached patrol point, wait
            agent.velocity = Vector3.zero;
            patrolWaitCounter += Time.deltaTime;
            
            if (patrolWaitCounter >= patrolWaitTime)
            {
                // Move to next patrol point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                patrolWaitCounter = 0f;
                GoToNextPatrolPoint();
            }
        }
    }
    
    private void HandleInvestigating()
    {
        float distToSound = Vector3.Distance(transform.position, soundInvestigationPoint);
        
        if (distToSound > 0.5f)
        {
            // Move toward sound source
            agent.SetDestination(soundInvestigationPoint);
        }
        else
        {
            // Reached sound source, wait and listen
            agent.velocity = Vector3.zero;
        }
        
        // Lose track timer
        loseTrackCounter += Time.deltaTime;
        if (loseTrackCounter >= loseTrackTime)
        {
            currentState = GuardState.Patrolling;
            patrolWaitCounter = 0f;
            GoToNextPatrolPoint();
        }
    }
    
    private void HandleChasing()
    {
        if (playerTarget == null)
        {
            currentState = GuardState.Patrolling;
            return;
        }
        
        // Chase the player
        agent.SetDestination(playerTarget.position);
        
        // Lose track timer
        loseTrackCounter += Time.deltaTime;
        if (loseTrackCounter >= loseTrackTime)
        {
            playerTarget = null;
            currentState = GuardState.Patrolling;
            patrolWaitCounter = 0f;
            GoToNextPatrolPoint();
        }
    }
    
    private bool CanSeePlayer(out Transform player)
    {
        player = null;
        
        // Find all objects in sight range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(sightOrigin.position, sightRange);
        
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                // Check if player is in cone
                Vector3 dirToPlayer = (col.transform.position - sightOrigin.position).normalized;
                float angleToPlayer = Vector3.Angle(sightOrigin.right, dirToPlayer);
                
                if (angleToPlayer < sightAngle / 2f)
                {
                    player = col.transform;
                    return true;
                }
            }
        }
        
        return false;
    }
    
    // Call this from EnemyListener when sound is detected
    public void OnPlayerSoundDetected(Vector3 soundPosition)
    {
        // Only switch to investigating if not already chasing
        if (currentState != GuardState.Chasing)
        {
            soundInvestigationPoint = soundPosition;
            currentState = GuardState.Investigating;
            loseTrackCounter = 0f;
        }
    }
    
    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw sight cone
        if (sightOrigin != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(sightOrigin.position, sightRange);
        }
        
        // Draw patrol points
        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, 0.3f);
            }
        }
    }
}