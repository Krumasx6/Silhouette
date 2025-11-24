using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    Animator anim;
    private Vector2 lastMoveDirection;
    private bool facingLeft = true;

    private PlayerAttributes attr;
    private Rigidbody2D rb;
    [SerializeField] private GameObject playerSoundObject;
    [SerializeField] private float footstepSpeed = 0.5f;
    private bool playingFootsteps = false;
    private float footstepTimer = 0f;
    
    private void Start()
    {
        attr = GetComponent<PlayerAttributes>();
        rb = GetComponent<Rigidbody2D>();
        attr.currentStamina = attr.maxStamina;
        anim = GetComponentInChildren<Animator>();
        
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
        HandleInput();
        HandleStamina();
        HandleBreathing();
        HandleFootsteps();
        Flip();
        Animate();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if ((moveX == 0 && moveY == 0) && (attr.input.x != 0 || attr.input.y != 0))
            lastMoveDirection = attr.input;

        attr.input = new Vector2(moveX, moveY);
        attr.moveInput = new Vector2(moveX, moveY).normalized;


        attr.isRunning = Input.GetKey(KeyCode.LeftShift) && attr.currentStamina > 0 && attr.input.magnitude > 0;
    }


    private void Animate()
    {
        anim.SetFloat("DirX", attr.input.x);
        anim.SetFloat("DirY", attr.input.y);
        anim.SetFloat("Speed", new Vector2(attr.input.x, attr.input.y).magnitude);

        anim.SetFloat("LatMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);

        anim.SetBool("isRunning", attr.isRunning);
        
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
    
    private void Flip()
    {
        // 1) If no horizontal input, keep the current facing direction
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
            return;

        // 2) Determine if player should face left or right
        bool shouldFaceLeft = rb.linearVelocity.x < 0f;

        // 3) Only flip if direction actually changed
        if (shouldFaceLeft != attr.isFacingLeft)
        {
            attr.isFacingLeft = shouldFaceLeft;

            // 4) Flip the sprite by changing localScale
            Vector3 scale = transform.localScale;
            scale.x = shouldFaceLeft ? -1f : 1f;
            transform.localScale = scale;
        }
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