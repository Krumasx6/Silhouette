using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    // ========== FACING DIRECTION ==========
    public bool isFacingRight;

    // ========== MOVEMENT INPUT ==========
    [Header("Movement Input")]
    [Tooltip("Horizontal input (-1 to 1)")]
    public float horizontal;
    [Tooltip("Vertical input (-1 to 1)")]
    public float vertical;

    // ========== WALKING & RUNNING ==========
    [Header("Movement Speed")]
    [Tooltip("Walking speed in units per second")]
    public float walkSpeed = 5f;
    [Tooltip("Running speed in units per second")]
    public float runSpeed = 8f;
    
    // ========== STAMINA SYSTEM ==========
    [Header("Stamina")]
    [Tooltip("Maximum stamina amount")]
    public float maxStamina = 100f;
    [Tooltip("Stamina drained per second while running")]
    public float staminaDrainRate = 20f;
    [Tooltip("Stamina regenerated per second when not running")]
    public float staminaRegenRate = 15f;
    [Tooltip("Delay in seconds before stamina starts regenerating")]
    public float staminaRegenDelay = 1f;
    [Tooltip("Stamina percentage threshold when heavy breathing starts")]
    public float heavyBreathingThreshold = 30f;
    
    // ========== AUDIO ==========
    [Header("Audio")]
    [Tooltip("Audio source for breathing sounds")]
    public AudioSource breathingAudioSource;
    [Tooltip("Audio clip for heavy breathing sound")]
    public AudioClip heavyBreathingClip;
    [Tooltip("Distance in units that guards can hear sbreathing")]
    public float breathingHearRadius = 10f;

    // ========== INTERNAL STATE ==========
    [Header("Internal State")]
    [Tooltip("Checking the state of the player")]
    public Vector2 moveInput;
    public bool isRunning;
    public float currentStamina;
    public bool canAttack;
    public bool notSeen;
    public float staminaRegenTimer;
    public bool isBreathingHeavily;
}