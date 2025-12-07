using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance { get; private set; }
    [SerializeField] SpawningDeadBodies spawnPrefabs;
    [SerializeField] EnemyAttributes currentEnemy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void GameOver()
    {
        
    }

    private void RoundComplete()
    {
        
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void EnemyKilled()
    {
        if (currentEnemy != null)
        {
            currentEnemy.isDead = true;
            spawnPrefabs.StartCoroutine(spawnPrefabs.SpawnDeadBody());
        }
    }

}
