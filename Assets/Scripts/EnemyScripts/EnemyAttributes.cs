using UnityEngine;

public class EnemyAttributes : MonoBehaviour
{

    [Header("Walk/Run Speeds")]
    [Tooltip("Walking speed")]
    public float walkSpeed = 1.5f;

    [Header("Chase Speed")]
    [Tooltip("Chasing speed")]
    public float chaseSpeed = 3.3f;

    [Header("Awareness")]
    [Tooltip("How long the enemy remains alert after losing sight of the player")]
    public float alertDuration = 5f;
    public bool heardAnything = false;
    public bool sawPlayer = false;
    public bool noticedSomething = false;

}
