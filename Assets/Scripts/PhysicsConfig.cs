using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PhysicsConfig : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        VerifyAndEnforceSettings();
    }

    private void VerifyAndEnforceSettings()
    {
        bool settingsChanged = false;

        // Enforce Rigidbody2D continuous collision detection
        if (rb.collisionDetectionMode != CollisionDetectionMode2D.Continuous)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            settingsChanged = true;
            Debug.LogWarning("[PhysicsConfig] Enforced Continuous collision detection on Rigidbody2D.", this);
        }

        // Enforce 0 Gravity (The ball is now driven purely by the Joystick, no falling)
        if (rb.gravityScale != 0f)
        {
            rb.gravityScale = 0f;
            settingsChanged = true;
            Debug.LogWarning("[PhysicsConfig] Enforced Gravity Scale to 0 on Rigidbody2D.", this);
        }

        // Prevent the ball from spinning when it hits walls
        if (!rb.freezeRotation)
        {
            rb.freezeRotation = true;
            settingsChanged = true;
            Debug.LogWarning("[PhysicsConfig] Enforced Freeze Rotation on Rigidbody2D.", this);
        }

        // Remove linear damping (drag) since we set velocity directly now
        if (rb.linearDamping != 0f)
        {
            rb.linearDamping = 0f;
            settingsChanged = true;
        }

        // Global gravity should also be 0 just in case
        if (Physics2D.gravity != Vector2.zero)
        {
            Physics2D.gravity = Vector2.zero;
        }

        // Check for Zero Friction Physics Material
        if (col.sharedMaterial == null)
        {
            Debug.LogWarning("[PhysicsConfig] Missing Physics Material 2D on the Collider! Attach a ZeroFriction material.", this);
        }

        if (settingsChanged)
        {
            Debug.Log("[PhysicsConfig] Physics settings were corrected for Joystick control.", this);
        }
    }
}
