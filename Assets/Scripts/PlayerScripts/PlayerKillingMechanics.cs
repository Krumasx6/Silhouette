using System.Collections;
using UnityEngine;

public class PlayerKillingMechanics : MonoBehaviour
{   
    private PlayerAttributes pa;
    private EnemyAttributes currentEnemy;
    private SpawningDeadBodies spawnPrefabs;
    [SerializeField] QTEManager qTEManager;
    [SerializeField] private GameObject qtePanel;
    
    private Animator playerAnimator;
    private Animator enemyAnimator;
    private Vector2 playerVelocityBeforeQTE;
    private Vector2 enemyVelocityBeforeQTE;
    private bool qteInProgress = false;

    void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
        playerAnimator = GetComponent<Animator>();
        
        // Get qtePanel from PlayerMovement if not assigned
        if (qtePanel == null)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null)
            {
                qtePanel = GameObject.Find("QTEPanel"); // Adjust if your panel has a different name
            }
        }
    }

    void Update()
    {
        if (pa.canAttack && Input.GetKeyDown(KeyCode.E) && !qteInProgress)
        {
            if (currentEnemy != null && !currentEnemy.isDead)
            {
                StartAttackQTE();
            }
        }
    }

    private void StartAttackQTE()
    {
        qteInProgress = true;
        pa.canAttack = false;
        
        // Activate QTE panel - this freezes BOTH player and enemy in their respective Update loops
        qtePanel.SetActive(true);
        
        // Subscribe to QTE completion
        qTEManager.OnQTEComplete += HandleQTEResult;
        
        // Start appropriate QTE difficulty
        if (currentEnemy.unaware)
        {
            qTEManager.StartEasyQTE();
        }
        else if (currentEnemy.isCautious)
        {
            qTEManager.StartMediumQTE();
        }
        else if (currentEnemy.isAlert)
        {
            qTEManager.StartHardQTE();
        }
    }

    private void HandleQTEResult(bool success)
    {
        Debug.Log("HandleQTEResult called with success: " + success);
        Debug.Log("currentEnemy before handling: " + (currentEnemy != null ? currentEnemy.gameObject.name : "NULL"));
        
        // Unsubscribe
        qTEManager.OnQTEComplete -= HandleQTEResult;
        
        if (success)
        {
            OnQTESuccess();
        }
        else
        {
            OnQTEFail();
        }
        
        qteInProgress = false;
    }

    private void OnQTESuccess()
    {
        Debug.Log("=== QTE SUCCESS CALLED ===");
        
        // DEACTIVATE QTE PANEL IMMEDIATELY - don't wait for QTEManager
        qtePanel.SetActive(false);
        Debug.Log("QTE Panel deactivated IMMEDIATELY");
        
        // Kill enemy and spawn dead body
        if (currentEnemy != null && spawnPrefabs != null)
        {
            currentEnemy.isDead = true;
            spawnPrefabs.StartCoroutine(spawnPrefabs.SpawnDeadBody());
            currentEnemy.gameObject.SetActive(false);
            currentEnemy = null;
            spawnPrefabs = null;
        }
        
        // Allow player to act again
        pa.canAttack = true;
        qteInProgress = false;
        Debug.Log("=== QTE SUCCESS FINISHED ===");
    }

    private void OnQTEFail()
    {
        // Deactivate QTE panel to unfreeze everything
        qtePanel.SetActive(false);
        
        // Allow player to act again
        pa.canAttack = true;
        qteInProgress = false;
        Debug.Log("QTE Failed - qteInProgress set to FALSE");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentEnemy = collision.GetComponent<EnemyAttributes>();
            spawnPrefabs = collision.GetComponent<SpawningDeadBodies>();
            
            if (currentEnemy != null)
            {
                pa.canAttack = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Don't clear during QTE
            if (qteInProgress)
            {
                Debug.Log("QTE in progress, not clearing currentEnemy");
                return;
            }
            
            pa.canAttack = false;
            currentEnemy = null;
            spawnPrefabs = null;
            Debug.Log("Left attack range");
        }
    }
}