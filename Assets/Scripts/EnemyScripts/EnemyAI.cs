using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float loseTrackTime = 6f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private GameObject qtePanel;

    private enum EnemyState { Patrolling, Investigating, Chasing, Dead }
    private EnemyState currentState = EnemyState.Patrolling;

    private EnemyAttributes ea;
    private EnemyPathing enemyPathing;
    private FieldOfView fieldOfView;
    private NavMeshAgent agent;
    private Animator animator;

    private int currentPatrolIndex = 0;
    private float patrolWaitCounter = 0f;
    private float loseTrackCounter = 0f;

    private Vector3 soundInvestigationPoint;
    private Transform playerTarget;

    private void Start()
    {
        enemyPathing = GetComponent<EnemyPathing>();
        fieldOfView = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ea = GetComponent<EnemyAttributes>();

        if (agent != null)
            agent.stoppingDistance = stoppingDistance;

        if (patrolPoints.Length > 0)
            SetTarget(patrolPoints[currentPatrolIndex]);
    }

    private void Update()
    {
        // ─── CHECK IF QTE IS ACTIVE (freeze if yes) ───────────────────
        if (qtePanel != null && qtePanel.activeSelf)
        {
            FreezeEnemy();
            return;
        }

        // ─── UNFREEZE if dead is false ───────────────────────────────
        if (!ea.isDead)
        {
            UnfreezeEnemy();
        }

        FaceDirection();

        // ─── UPDATE ALERT STATES (based on detection) ────────────────
        UpdateAlertStates();

        // ─── 1) CHASING has highest priority ───────────────────────
        if (fieldOfView.visiblePlayer.Count > 0 && ea.sawPlayer)
        {
            playerTarget = fieldOfView.visiblePlayer[0];
            SetTarget(playerTarget);
            SetState(EnemyState.Chasing);
            loseTrackCounter = 0f;
            return;
        }

        // ─── Player lost from sight ────────────────────────────────
        if (currentState == EnemyState.Chasing && fieldOfView.visiblePlayer.Count == 0)
            playerTarget = null;

        // ─── 2) Handle active state ───────────────────────────────
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

    // ───────────────────────────────────────────────────────────────
    // UPDATE ALERT STATES
    // ───────────────────────────────────────────────────────────────
    private void UpdateAlertStates()
    {
        // Heard something = Cautious
        if (ea.heardAnything)
        {
            ea.isCautious = true;
            ea.isAlert = false;
            ea.unaware = false;
        }
        // Saw player = Alert
        else if (ea.sawPlayer)
        {
            ea.isAlert = true;
            ea.isCautious = false;
            ea.unaware = false;
        }
        // Neither = Unaware
        else
        {
            ea.unaware = true;
            ea.isCautious = false;
            ea.isAlert = false;
        }
    }

    // ───────────────────────────────────────────────────────────────
    // FREEZE DURING QTE
    // ───────────────────────────────────────────────────────────────
    private void FreezeEnemy()
    {
        if (agent != null)
        {
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.speed = 0f;
        }
    }

    private void UnfreezeEnemy()
    {
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    // ───────────────────────────────────────────────────────────────
    // SET STATE (your booleans update here, nowhere else)
    // ───────────────────────────────────────────────────────────────
    private void SetState(EnemyState newState)
    {
        currentState = newState;

        ea.isPatrolling = (newState == EnemyState.Patrolling);
        ea.isInvestigating = (newState == EnemyState.Investigating);
        ea.isChasing = (newState == EnemyState.Chasing);
        ea.isDead = (newState == EnemyState.Dead);

        // 🔥 FIX: reset patrol timer when leaving Patrolling
        if (newState != EnemyState.Patrolling)
        {
            patrolWaitCounter = 0f;
        }
    }

    // ───────────────────────────────────────────────────────────────
    // PATROLLING
    // ───────────────────────────────────────────────────────────────
    private void HandlePatrolling()
    {
        if (patrolPoints.Length == 0)
            return;

        if (agent.remainingDistance <= stoppingDistance && !agent.hasPath)
        {
            patrolWaitCounter += Time.deltaTime;

            if (patrolWaitCounter >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                patrolWaitCounter = 0f;
                SetTarget(patrolPoints[currentPatrolIndex]);
            }
        }
        else
        {
            patrolWaitCounter = 0f;
        }
    }

    // ───────────────────────────────────────────────────────────────
    // INVESTIGATING (sound)
    // ───────────────────────────────────────────────────────────────
    private void HandleInvestigating()
    {
        float dist = Vector3.Distance(transform.position, soundInvestigationPoint);

        loseTrackCounter += Time.deltaTime;

        if (loseTrackCounter >= loseTrackTime)
        {
            SetState(EnemyState.Patrolling);
            patrolWaitCounter = 0f;
            SetTarget(patrolPoints[currentPatrolIndex]);
        }
    }

    // ───────────────────────────────────────────────────────────────
    // CHASING
    // ───────────────────────────────────────────────────────────────
    private void HandleChasing()
    {
        if (playerTarget == null)
        {
            SetState(EnemyState.Patrolling);
            SetTarget(patrolPoints[currentPatrolIndex]);
            return;
        }

        loseTrackCounter += Time.deltaTime;

        if (loseTrackCounter >= loseTrackTime)
        {
            playerTarget = null;
            SetState(EnemyState.Patrolling);
            SetTarget(patrolPoints[currentPatrolIndex]);
        }
    }

    // ───────────────────────────────────────────────────────────────
    // SOUND EVENT from EnemyListener
    // ───────────────────────────────────────────────────────────────
    public void OnPlayerSoundDetected(Vector3 soundPos)
    {
        if (currentState == EnemyState.Chasing)
            return; // ignore sound when already chasing

        ea.heardAnything = true;
        soundInvestigationPoint = soundPos;
        loseTrackCounter = 0f;

        enemyPathing.SetTargetPosition(soundPos);

        SetState(EnemyState.Investigating);
    }

    private void SetTarget(Transform target)
    {
        if (target != null)
            enemyPathing.SetTargetTransform(target);
    }

    private void FaceDirection()
    {
        if (agent.velocity.magnitude < 0.1f)
            return;

        Vector3 dir = agent.velocity.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}