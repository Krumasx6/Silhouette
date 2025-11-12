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

        Vector2 eyePosition = (Vector2)transform.position + (Vector2)transform.right * 0.4f + new Vector2(0, eyeOffset);

        float proximityRadius = 0.8f;
        Collider2D closeHit = Physics2D.OverlapCircle(eyePosition, proximityRadius, playerMask);
        if (closeHit != null)
        {
            playerInSight = true;
            visiblePlayer.Add(closeHit.transform);
            return; // Exit early, no need to run the cone logic
        }
        
        playerInRadius = Physics2D.OverlapCircleAll(eyePosition, viewRadius, playerMask);
        for (int i = 0; i < playerInRadius.Length; i++)
        {
            Transform player = playerInRadius[i].transform;

            if (Vector2.Distance(player.position, eyePosition) < 0.7f)
            {
                playerInSight = true;
                visiblePlayer.Add(player);
                continue; // Skip to next player, already detected
            }
            
            // Check if player is within view angle
            Vector2 dirToCenter = new Vector2(player.position.x - eyePosition.x, player.position.y - eyePosition.y);
           if (Vector2.Angle(dirToCenter, transform.right) < (viewAngle / 2) + 5f)
            {
                // Get player's collider bounds
                Collider2D playerCollider = playerInRadius[i];
                Bounds bounds = playerCollider.bounds;
                
                // Check multiple points on the player's collider
                bool canSeePlayer = false;
                List<Vector2> checkPoints = new List<Vector2>();
                
                // Add center point
                checkPoints.Add(player.position);
                
                int resolution = 3; // or 4 for denser coverage
                for (int x = 0; x < resolution; x++)
                {
                    for (int y = 0; y < resolution; y++)
                    {
                        float px = Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / (resolution - 1));
                        float py = Mathf.Lerp(bounds.min.y, bounds.max.y, (float)y / (resolution - 1));
                        checkPoints.Add(new Vector2(px, py));
                    }
                }

                
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