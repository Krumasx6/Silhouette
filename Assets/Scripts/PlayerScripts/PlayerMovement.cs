using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttributes attr;
    private Rigidbody2D rb;
    [SerializeField] private GameObject playerSoundObject;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    
    private void Start()
    {
        attr = GetComponent<PlayerAttributes>();
        rb = GetComponent<Rigidbody2D>();
        attr.currentStamina = attr.maxStamina;
        
        // Setup audio source for breathing
        if (attr.breathingAudioSource == null)
        {
            attr.breathingAudioSource = gameObject.AddComponent<AudioSource>();
        }
        attr.breathingAudioSource.loop = true;
        attr.breathingAudioSource.spatialBlend = 1f; // 3D sound
        attr.breathingAudioSource.maxDistance = attr.breathingHearRadius;
    }
    
    private void Update()
    {
        HandleInput();
        HandleStamina();
        HandleBreathing();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleInput()
    {
        // Get movement input (WASD or Arrow keys)
        attr.horizontal = Input.GetAxisRaw("Horizontal");
        attr.vertical = Input.GetAxisRaw("Vertical");
        attr.moveInput = new Vector2(attr.horizontal, attr.vertical);
        attr.moveInput.Normalize(); // Prevent faster diagonal movement
        
        // Check if player wants to run (Left Shift)
        attr.isRunning = Input.GetKey(KeyCode.LeftShift) && attr.currentStamina > 0 && attr.moveInput.magnitude > 0;

        if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
        {   
            StartFootsteps();
        }
        else if (rb.linearVelocity.magnitude < 0.01f && playingFootsteps)
        {
            StopFootsteps();
        }
    }
    
    private void HandleMovement()
    {
        float currentSpeed = attr.isRunning ? attr.runSpeed : attr.walkSpeed;
        Vector2 velocity = attr.moveInput * currentSpeed;
        rb.linearVelocity = velocity;

    }
    
    private void HandleStamina()
    {
        if (attr.isRunning)
        {
            // Drain stamina while running
            playerSoundObject.SetActive(true);
            attr.currentStamina -= attr.staminaDrainRate * Time.deltaTime;
            attr.currentStamina = Mathf.Max(0, attr.currentStamina);
            attr.staminaRegenTimer = attr.staminaRegenDelay;
        }
        else if (attr.currentStamina < attr.maxStamina)
        {
            
            playerSoundObject.SetActive(false);
            // Regenerate stamina after delay
            if (attr.staminaRegenTimer > 0)
            {
                attr.staminaRegenTimer -= Time.deltaTime;
            }
            else
            {
                attr.currentStamina += attr.staminaRegenRate * Time.deltaTime;
                attr.currentStamina = Mathf.Min(attr.maxStamina, attr.currentStamina);
            }
        }
    }
    
    private void HandleBreathing()
    {
        float staminaPercentage = (attr.currentStamina / attr.maxStamina) * 100f;
        
        // Start heavy breathing when stamina is low
        if (staminaPercentage < attr.heavyBreathingThreshold && !attr.isBreathingHeavily)
        {
            StartHeavyBreathing();
        }
        // Stop heavy breathing when stamina recovers
        else if (staminaPercentage >= attr.heavyBreathingThreshold + 10f && attr.isBreathingHeavily)
        {
            StopHeavyBreathing();
        }
        
        // Adjust breathing intensity based on stamina
        if (attr.isBreathingHeavily && attr.breathingAudioSource.isPlaying)
        {
            float intensity = 1f - (staminaPercentage / attr.heavyBreathingThreshold);
            attr.breathingAudioSource.volume = Mathf.Clamp01(intensity);
        }
    }
    
    private void StartHeavyBreathing()
    {
        if (attr.heavyBreathingClip != null && !attr.breathingAudioSource.isPlaying)
        {
            attr.isBreathingHeavily = true;
            attr.breathingAudioSource.clip = attr.heavyBreathingClip;
            attr.breathingAudioSource.Play();
        }
    }
    
    private void StopHeavyBreathing()
    {
        attr.isBreathingHeavily = false;
        attr.breathingAudioSource.Stop();
    }
    
    // Public methods for UI or other systems
    public float GetStaminaPercentage()
    {
        return (attr.currentStamina / attr.maxStamina) * 100f;
    }
    
    public bool IsBreathingHeavily()
    {
        return attr.isBreathingHeavily;
    }
    
    public float GetBreathingHearRadius()
    {
        return attr.breathingHearRadius;
    }
    
    // For guards to check if they can hear the player
    public bool CanBeHeardAt(Vector3 listenerPosition)
    {
        if (!attr.isBreathingHeavily) return false;
        
        float distance = Vector3.Distance(transform.position, listenerPosition);
        return distance <= attr.breathingHearRadius;
    }

    void PlayBackgroundMusic()
    {
        SoundEffectManager.Play("BackgroundMusic");
    }
    
    // ===== Footstep Sound Management =====
    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    } 

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep");
    }
}