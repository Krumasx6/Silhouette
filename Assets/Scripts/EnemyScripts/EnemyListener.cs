using UnityEditor;
using UnityEngine;

public class EnemyListener : MonoBehaviour
{
    [SerializeField] private GameObject prefabPlayerSounds;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSounds"))
        {
            Debug.Log("Enemy heard player sounds.");
        }
    }

}