using UnityEngine;
using UnityEngine.InputSystem;

public class RingController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed at which the ring rotates.")]
    [SerializeField] private float rotationSpeed = 150f;
    
    [Tooltip("Whether the ring can be rotated via mouse/touch drag.")]
    [SerializeField] private bool allowMouseDrag = true;

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
        Debug.Log($"[RingController] Ring rotation is enabled. Maze generated at '{gameObject.name}'.");
    }

    private void Update()
    {
        float input = 0f;

        // Check for keyboard input (A/D or Left/Right arrow keys)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                input = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                input = 1f;
            }
        }
        
        // Alternatively, check for mouse or touch drag
        if (allowMouseDrag)
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                // Read horizontal mouse movement (delta) and scale it down 
                // since pointer deltas are larger than old Input.GetAxis
                input = Mouse.current.delta.x.ReadValue() * 0.1f;
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                input = Touchscreen.current.primaryTouch.delta.x.ReadValue() * 0.1f;
            }
        }
        
        // Apply rotation if there is any input
        if (Mathf.Abs(input) > 0.01f)
        {
            // Rotate the ring in the same direction as the input
            transform.Rotate(Vector3.forward, input * rotationSpeed * Time.deltaTime);
        }
    }
}
