using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    [Header("Components")]
    [SerializeField] private Animator anim;
    private PlayerAttributes attr;
    private Rigidbody2D rb;
    
    [Header("Audio")]
    [SerializeField] private GameObject playerSoundObject;
    [SerializeField] private float footstepSpeed = 0.5f;
    private bool playingFootsteps = false;
    private float footstepTimer = 0f;
    
    [Header("Movement State")]
    private Vector2 input;
    private Vector2 lastMoveDirection;
    private bool facingRight = true;
    
    private void Start()
    {
        attr = GetComponent<PlayerAttributes>();
        rb = GetComponent<Rigidbody2D>();
        
        attr.currentStamina = attr.maxStamina;
        lastMoveDirection = Vector2.down;
        
        // Setup audio source for breathing
        if (attr.breathingAudioSource == null)
        {
            attr.breathingAudioSource = gameObject.AddComponent<AudioSource>();
        }
        attr.breathingAudioSource.loop = true;
        attr.breathingAudioSource.spatialBlend = 1f;
        attr.breathingAudioSource.maxDistance = attr.breathingHearRadius;
    }
    
    private void Update()
    {
        ProcessInputs();
        HandleStamina();
        HandleBreathing();
        HandleFootsteps();
        Animate();
        
        // Flip only when horizontal movement present
        if (input.x > 0 && !facingRight) Flip();
        else if (input.x < 0 && facingRight) Flip();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Store lastMoveDirection only when input stops
        if (moveX == 0f && moveY == 0f && (input.x != 0f || input.y != 0f))
        {
            lastMoveDirection = input;
        }

        input.x = moveX;
        input.y = moveY;
        input = input.normalized;

        // Running logic
        attr.isRunning = Input.GetKey(KeyCode.LeftShift) && attr.currentStamina > 0 && input.magnitude > 0;
    }

    private void Animate()
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetBool("isRunning", attr.isRunning);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }
    
    private void HandleMovement()
    {
        float currentSpeed = attr.isRunning ? attr.runSpeed : attr.walkSpeed;
        rb.linearVelocity = input * currentSpeed;
    }
    
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
        facingRight = !facingRight;
        attr.isFacingRight = facingRight;
    }
    
    private void HandleStamina()
    {
        if (attr.isRunning)
        {
            if (playerSoundObject != null)
            {
                playerSoundObject.SetActive(true);
            }
            attr.currentStamina -= attr.staminaDrainRate * Time.deltaTime;
            attr.currentStamina = Mathf.Max(0, attr.currentStamina);
            attr.staminaRegenTimer = attr.staminaRegenDelay;
        }
        else if (attr.currentStamina < attr.maxStamina)
        {
            if (playerSoundObject != null)
            {
                playerSoundObject.SetActive(false);
            }
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
        
        if (staminaPercentage < attr.heavyBreathingThreshold && !attr.isBreathingHeavily)
        {
            StartHeavyBreathing();
        }
        else if (staminaPercentage >= attr.heavyBreathingThreshold + 10f && attr.isBreathingHeavily)
        {
            StopHeavyBreathing();
        }
        
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
    
    private void HandleFootsteps()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        
        if (isMoving)
        {
            if (!playingFootsteps)
            {
                PlayFootstep();
                float currentFootstepSpeed = attr.isRunning ? footstepSpeed * 0.6f : footstepSpeed;
                footstepTimer = currentFootstepSpeed;
                playingFootsteps = true;
            }
            else
            {
                footstepTimer -= Time.deltaTime;
                
                if (footstepTimer <= 0f)
                {
                    PlayFootstep();
                    float currentFootstepSpeed = attr.isRunning ? footstepSpeed * 0.6f : footstepSpeed;
                    footstepTimer = currentFootstepSpeed;
                }
            }
        }
        else
        {
            playingFootsteps = false;
            footstepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep");
    }
    
    void PlayBackgroundMusic()
    {
        SoundEffectManager.Play("BackgroundMusic");
    }
    
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
    
    public bool CanBeHeardAt(Vector3 listenerPosition)
    {
        if (!attr.isBreathingHeavily) return false;
        
        float distance = Vector3.Distance(transform.position, listenerPosition);
        return distance <= attr.breathingHearRadius;
    }
}