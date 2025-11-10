using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerAttributes pa;

    void Update()
    {
        // Movement blend
        anim.SetFloat("Speed", Mathf.Abs(pa.horizontal));

        // Running trigger
        anim.SetBool("isRunning", pa.isRunning);

        // Grounded trigger (will be used when you add jump)
        anim.SetBool("onGround", pa.onGround);

        // Optional: crouching if you have crouch animation
        anim.SetBool("isCrouching", pa.isCrouching);
    }
}

