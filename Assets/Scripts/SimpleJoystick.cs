using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Tooltip("The inner circle (handle) of the joystick.")]
    [SerializeField] private RectTransform handle;

    [Tooltip("How far the handle can move from the center.")]
    [SerializeField] private float handleRange = 100f;

    private RectTransform background;
    private Vector2 inputVector = Vector2.zero;

    // The local-space position where the finger first touched down.
    // All drag movement is computed as a delta from this origin so that
    // tapping and holding at the center produces exactly (0,0).
    private Vector2 touchOrigin;

    // This is the value the BallController will read
    public Vector2 Direction => inputVector;

    private void Awake()
    {
        background = GetComponent<RectTransform>();
        if (handle == null)
        {
            Debug.LogError("[SimpleJoystick] Handle is not assigned! Please assign the inner circle RectTransform.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (background == null) return;

        // Record where the finger first touched (in local space).
        // This becomes the "zero point" — dragging is measured from here.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out touchOrigin
        );

        // On initial press the handle stays at center (no movement yet)
        inputVector = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        // Convert the current screen position to local space
        Vector2 currentLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, 
            eventData.position, 
            eventData.pressEventCamera, 
            out currentLocal
        );

        // Calculate the delta from the initial touch point
        Vector2 delta = currentLocal - touchOrigin;

        // Normalize the delta based on the handle range (so it's between -1 and 1)
        Vector2 normalized = delta / handleRange;

        // Ensure the input doesn't exceed a magnitude of 1 (keeps it circular)
        inputVector = (normalized.magnitude > 1.0f) ? normalized.normalized : normalized;

        // Move the handle visually
        handle.anchoredPosition = inputVector * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset joystick when released
        inputVector = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
