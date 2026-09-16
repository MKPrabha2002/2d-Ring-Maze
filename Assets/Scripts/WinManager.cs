using UnityEngine;

public class WinManager : MonoBehaviour
{
    [Tooltip("A GameObject to activate when the player wins (e.g., a particle effect, UI text, etc.).")]
    [SerializeField] private GameObject winEffect;

    [Tooltip("Maximum distance from center (0,0) for the win to count.")]
    [SerializeField] private float winRadius = 0.5f;

    private bool hasWon = false;

    private void Awake()
    {
        // RUNTIME ENFORCEMENT: Ensure the collider on this object is set as a trigger.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("[WinManager] Collider was not set as Trigger! Fixed automatically at runtime.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckWinCondition(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckWinCondition(other);
    }

    private void CheckWinCondition(Collider2D other)
    {
        if (hasWon) return; // Already won, ignore further triggers

        if (other.CompareTag("Player"))
        {
            // SAFETY CHECK: Only count a win if the ball is actually near the center of the maze
            float distanceFromCenter = Vector2.Distance(other.transform.position, transform.position);
            
            if (distanceFromCenter > winRadius)
            {
                // Ball hit a trigger but is NOT near the center — ignore it
                return;
            }

            hasWon = true;

            // 1. Stop the ball's movement and disable player control
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            BallController ballController = other.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.enabled = false;
            }

            // 2. Stop all wind simulation by zeroing global gravity
            Physics2D.gravity = Vector2.zero;

            // 3. Activate the visual win effect
            if (winEffect != null)
            {
                winEffect.SetActive(true);
            }

            Debug.Log("[WinManager] *** YOU WIN! Ball reached the center! ***");
        }
    }
}
