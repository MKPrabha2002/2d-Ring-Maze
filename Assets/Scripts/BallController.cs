using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Switch to Dynamic + Simulated so physics works during gameplay
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        // Force both the Rigidbody and Transform to the desired position.
        // Directly setting rb.position is required to override any cached physics state.
        Vector2 targetPos = new Vector2(-3.15f, -0.5f);
        rb.position = targetPos;
        transform.localPosition = new Vector3(targetPos.x, targetPos.y, 0f);
    }
}
