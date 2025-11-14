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

    // ========== MOVEMENT PHYSICS ==========
    [Header("Movement Physics")]
    [Tooltip("Default gravity scale")]
    public float defaultGravity = 5f;
    [Tooltip("Acceleration when on ground (units/s²)")]
    public float groundAcceleration = 60f;
    [Tooltip("Deceleration (friction) when on ground (units/s²)")]
    public float groundDeceleration = 80f;
    [Tooltip("Acceleration while in air (lower for less air control)")]
    public float airAcceleration = 20f;
    [Tooltip("Deceleration while in air")]
    public float airDeceleration = 20f;
    
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
    [Tooltip("Distance in units that guards can hear breathing")]
    public float breathingHearRadius = 10f;

    // ========== INTERNAL STATE ==========
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool isRunning;
     public float currentStamina;
    [HideInInspector] public float staminaRegenTimer;
    [HideInInspector] public bool isBreathingHeavily;
}