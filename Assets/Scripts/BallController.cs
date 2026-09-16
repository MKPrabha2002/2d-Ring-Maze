using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Tooltip("Reference to the on-screen joystick.")]
    [SerializeField] private SimpleJoystick joystick;

    [Tooltip("How fast the ball moves.")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Switch to Dynamic + Simulated so physics works during gameplay
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        // Force both the Rigidbody and Transform to the desired position.
        // Directly setting rb.position is required to override any cached physics state.
        Vector2 targetPos = new Vector2(2.78f, 0f);
        rb.position = targetPos;
        transform.localPosition = new Vector3(targetPos.x, targetPos.y, 0f);

        if (joystick == null)
        {
            Debug.LogWarning("[BallController] Joystick reference is missing! Please assign it in the Inspector.");
        }
    }

    private void FixedUpdate()
    {
        if (joystick != null)
        {
            Vector2 direction = joystick.Direction;
            rb.linearVelocity = direction * moveSpeed;
        }
    }
}
