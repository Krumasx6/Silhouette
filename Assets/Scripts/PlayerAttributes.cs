using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public bool onGround = true;
    public bool isJumping = false;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public float airTime = 0.2f;
    public float horizontal;
}
