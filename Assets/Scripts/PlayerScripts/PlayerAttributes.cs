//PlayerAttributes
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public bool isFacingRight;

    // ========== GROUND STATE ==========
    // Tracks if player is touching ground
    public bool onGround = true;
    public bool isJumping = false;

    // ========== JUMPING ==========
    // Force applied when jumping
    public float jumpForce = 10f;
    // Duration of reduced gravity during jump rise
    public float airTime = 0.2f;

    // ========== MOVEMENT INPUT ==========
    // Horizontal input (-1 to 1)
    public float horizontal;
    // Vertical input for ladder climbing (-1 to 1)
    public float vertical;

    // ========== WALKING & RUNNING ==========
    [Header("Walk/Run Speeds")]
    [Tooltip("Walking speed")]
    public float walkSpeed = 4f;
    [Tooltip("Running speed (or set via walkSpeed * runMultiplier)")]
    public float runSpeed = 8f;
    
    // Current running state
    public bool isRunning = false;

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

    // ========== CROUCHING ==========
    [Header("Crouching")]
    [Tooltip("Movement speed while crouching")]
    public float crouchSpeed = 2f;
    [Tooltip("Minimum seconds between crouch toggles")]
    public float crouchToggleCooldown = 0.2f;
    
    // Current crouch state
    public bool isCrouching;
    // Last time crouch was toggled
    [HideInInspector] public float lastCrouchToggleTime = -10f;

    // ========== LADDER CLIMBING ==========
    [Header("Ladder Climbing")]
    [Tooltip("Speed when climbing ladders")]
    public float ladderX;
    public float climbSpeed = 4f;

    // Currently on a ladder trigger
    public bool isLadder;
    public bool canClimb;
    // Currently climbing (moving on ladder)
    public bool isClimbing;

    // ========== SLIDING ==========
    [Header("Sliding")]
    [Tooltip("Speed when slide starts")]
    public float slideSpeed = 10f;
    [Tooltip("How long the slide lasts (seconds)")]
    public float slideDuration = 0.5f;
    [Tooltip("How quickly the slide slows down (1 = no slowdown, smaller = faster slowdown)")]
    public float slideFriction = 0.98f;
    [Tooltip("Cooldown between slides (seconds)")]
    public float slideCooldown = 0.5f;
    [Tooltip("Percentage of run speed needed to slide (0-1, default 0.8 = 80%)")]
    [Range(0f, 1f)]
    public float slideSpeedThreshold = 0.8f;
    
    // Current sliding state
    public bool isSliding = false;
    // Last time slide was triggered
    [HideInInspector] public float lastSlideTime = -10f;


    // ========== VAULTING ==========
    [Header("Vaulting")]
    [Tooltip("Speed when vaulting over an obstacle")]
    public float vaultSpeed = 6f;
    // Current vaulting state
    public bool isVaulting = false;
    public bool isInVaultZone = false;
    public bool canVault = false;

}