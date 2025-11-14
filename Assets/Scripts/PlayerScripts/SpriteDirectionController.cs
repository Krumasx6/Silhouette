using UnityEngine;

public class SpriteDirectionController : MonoBehaviour
{
    public PlayerAttributes playerAttr;
    public Animator anim;

    private Vector2 lastDir = Vector2.down;

    void Update()
    {
        Vector2 input = playerAttr.moveInput;
        float speed = input.sqrMagnitude;

        if (speed > 0.01f)
        {
            lastDir = input.normalized;
        }

        Vector2 dir = speed > 0.01f ? input.normalized : lastDir;

        anim.SetFloat("DirX", dir.x);
        anim.SetFloat("DirY", dir.y);

        // freeze animation when idle
        anim.speed = speed < 0.01f ? 0f : 1f;

        anim.SetFloat("Speed", speed);
    }
}
