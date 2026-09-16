using UnityEngine;

public class RingController : MonoBehaviour
{
    // The maze is now completely static. 
    // All rotation logic and gravity manipulation has been removed.
    // The ball is now controlled directly via the UI Joystick (BallController.cs).

    private void Awake()
    {
        // Fix for the physics bug: OuterRing has a rogue solid CircleCollider2D
        // that physically ejects the ball to x=3.6 when simulation starts.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.enabled = false;
            Debug.Log("[RingController] Disabled rogue solid CircleCollider2D on OuterRing to prevent physics bug.", this);
        }
    }

    private void Start()
    {
        Debug.Log($"[RingController] Ring is static. Maze generated at '{gameObject.name}'.");
    }
}
