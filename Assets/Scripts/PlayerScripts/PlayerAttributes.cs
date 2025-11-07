using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public bool onGround = true;
    public bool isJumping = false;
    public bool isCrouching;
    public float jumpForce = 10f;

    public float airTime = 0.2f;
    public float horizontal;


    //Running/Walking

    [Header("Run/Wing")]
    [Tooltip("Multiplier applied to moveSpeed while holding Shift to run")]
    [SerializeField] public float runMultiplier = 1.6f;

    public bool isRunning = false;
    public float walkSpeed = 4f;
    public float runSpeed = 6.5f; // or walkSpeed * runMultiplier

    public float defaultMoveSpeed = 6f;
    public float defaultGravity = 5f;
    [Header("Movement")]
    [Tooltip("Top horizontal speed")]
    public float moveSpeed = 6f; // fallback if PlayerAttributes doesn't provide one
    [Tooltip("Acceleration when on ground (units/s^2)")]
    public float groundAcceleration = 60f;
    [Tooltip("Deceleration (friction) when on ground (units/s^2)")]
    public float groundDeceleration = 80f;
    [Tooltip("Acceleration while in air (lower for less control in air)")]
    public float airAcceleration = 20f;
    [Tooltip("Deceleration while in air")]
    public float airDeceleration = 20f;

    //Crouching
    [SerializeField, Tooltip("Min seconds between crouch toggles")] 
    public float crouchToggleCooldown = 0.2f;
    public float lastCrouchToggleTime = -10f;


}
