using UnityEngine;
using UnityEngine.AI;

public class EnemyPathing : MonoBehaviour
{
    private Transform target;
    private Vector3 targetPosition;
    private bool useTransformTarget = true;
    
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.5f; // Match this with EnemyAI's stoppingDistance
    }

    private void Update()
    {
        if (useTransformTarget && target != null)
        {
            agent.SetDestination(target.position);
        }
        else if (!useTransformTarget)
        {
            agent.SetDestination(targetPosition);
        }
    }

    public void SetTargetTransform(Transform newTarget)
    {
        target = newTarget;
        useTransformTarget = true;
        if (agent != null && target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        useTransformTarget = false;
        if (agent != null)
        {
            agent.SetDestination(newPosition);
        }
    }
}