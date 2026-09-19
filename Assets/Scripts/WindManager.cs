using UnityEngine;

public class WindManager : MonoBehaviour
{
    [Tooltip("How strong the wind pushes the ball.")]
    [SerializeField] private float windForce = 15f;

    [Tooltip("The Rigidbody2D of the ball. Will auto-find if left empty.")]
    [SerializeField] private Rigidbody2D ballRigidbody;

    private void Start()
    {
        // Automatically find the ball if it's not assigned
        if (ballRigidbody == null)
        {
            BallController ball = FindAnyObjectByType<BallController>();
            if (ball != null)
            {
                ballRigidbody = ball.GetComponent<Rigidbody2D>();
            }
        }
    }

    private void FixedUpdate()
    {
        if (ballRigidbody != null)
        {
            // Push the ball in the "upper" direction of this air pump
            // Since it's rotatable, transform.up changes as you rotate the pump
            Vector2 windDirection = transform.up;
            ballRigidbody.AddForce(windDirection * windForce);
        }
    }
}