using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 5;
    public float viewAngle = 135;
    public float eyeOffset = 0.45f;
    Collider2D[] playerInRadius;
    public LayerMask obstacleMask, playerMask;
    public List<Transform> visiblePlayer = new List<Transform>();
    public bool playerInSight;
    [SerializeField] private int edgeResolveIterations = 3; // Number of points to check on player collider

    void FixedUpdate()
    {
        FindVisiblePlayer();
    }

    void FindVisiblePlayer()
    {
        visiblePlayer.Clear();
        playerInSight = false;
        
        Vector2 eyePosition = new Vector2(transform.position.x, transform.position.y + eyeOffset);
        
        playerInRadius = Physics2D.OverlapCircleAll(eyePosition, viewRadius, playerMask);
        for (int i = 0; i < playerInRadius.Length; i++)
        {
            Transform player = playerInRadius[i].transform;
            
            // Check if player is within view angle
            Vector2 dirToCenter = new Vector2(player.position.x - eyePosition.x, player.position.y - eyePosition.y);
            if (Vector2.Angle(dirToCenter, transform.right) < viewAngle / 2)
            {
                // Get player's collider bounds
                Collider2D playerCollider = playerInRadius[i];
                Bounds bounds = playerCollider.bounds;
                
                // Check multiple points on the player's collider
                bool canSeePlayer = false;
                List<Vector2> checkPoints = new List<Vector2>();
                
                // Add center point
                checkPoints.Add(player.position);
                
                // Add edge points (left, right, top, bottom)
                checkPoints.Add(new Vector2(bounds.min.x, player.position.y)); // Left
                checkPoints.Add(new Vector2(bounds.max.x, player.position.y)); // Right
                checkPoints.Add(new Vector2(player.position.x, bounds.max.y)); // Top
                checkPoints.Add(new Vector2(player.position.x, bounds.min.y)); // Bottom
                
                // Add corner points for better detection
                checkPoints.Add(new Vector2(bounds.min.x, bounds.min.y)); // Bottom-left
                checkPoints.Add(new Vector2(bounds.max.x, bounds.min.y)); // Bottom-right
                checkPoints.Add(new Vector2(bounds.min.x, bounds.max.y)); // Top-left
                checkPoints.Add(new Vector2(bounds.max.x, bounds.max.y)); // Top-right
                
                // Check if any point is visible
                foreach (Vector2 point in checkPoints)
                {
                    Vector2 dirToPoint = point - eyePosition;
                    float distanceToPoint = dirToPoint.magnitude;
                    
                    // Only check if point is within view angle
                    if (Vector2.Angle(dirToPoint, transform.right) < viewAngle / 2)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(eyePosition, dirToPoint.normalized, distanceToPoint, obstacleMask);
                        
                        if (hit.collider == null)
                        {
                            canSeePlayer = true;
                            break;
                        }
                    }
                }
                
                if (canSeePlayer)
                {
                    visiblePlayer.Add(player);
                    playerInSight = true;
                }
            }
        }
    }

    public Vector2 DirFromAngle(float angleDeg, bool global)
    {
        if (!global)
        {
            angleDeg += transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
    }
    
    public Vector3 GetEyePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + eyeOffset, transform.position.z);
    }
}