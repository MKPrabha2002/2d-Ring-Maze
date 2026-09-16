using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Configuration")]
    [Tooltip("Number of concentric wall rings inside the outer boundary.")]
    public int numberOfRings = 4;

    [Tooltip("Physical width of each gap (local space). Keeps gap sizes consistent on inner rings.")]
    [SerializeField] private float gapWidth = 0.15f;

    [Tooltip("Radius of the outer boundary wall (local space). 0.48 = just inside a unit circle sprite edge.")]
    public float outerRadius = 0.48f;

    [Tooltip("Radius of the innermost win zone (local space).")]
    public float innerRadius = 0.08f;

    [Tooltip("Number of line segments per ring arc. Higher = smoother circles.")]
    [SerializeField] private int segmentsPerRing = 64;

    [Header("Hierarchy Configuration")]
    [Tooltip("The static container where maze walls will be parented to prevent them from rotating.")]
    [SerializeField] private Transform mazeContainer;

    // Gap angles for each ring — staggered to create a navigable maze path
    private readonly float[] gapAngles = { 0f, 180f, 90f, 270f, 45f, 225f, 135f, 315f };

    private void Awake()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        // 1. Generate the outer boundary wall (full closed circle — no gap, ball starts inside)
        CreateRingWall("OuterBoundary", outerRadius, 0f, 360f, segmentsPerRing, true);
        Debug.Log($"[MazeGenerator] Created outer boundary at radius {outerRadius}");

        // 2. Calculate spacing between rings
        float usableRadius = outerRadius - innerRadius;
        float radiusStep = usableRadius / (numberOfRings + 1);

        // 3. Generate each concentric ring wall with a staggered gap
        for (int i = 0; i < numberOfRings; i++)
        {
            float radius = outerRadius - (radiusStep * (i + 1));

            // Pick a gap angle from the stagger pattern (wraps around if more rings than angles)
            float gapCenter = gapAngles[i % gapAngles.Length];

            // Calculate the gap angle in degrees to maintain a constant physical gap width across all rings
            float currentGapAngleDeg = (gapWidth / radius) * Mathf.Rad2Deg;

            // Arc goes from (gapEnd) all the way around to (gapStart), leaving the gap open
            float arcStart = gapCenter + (currentGapAngleDeg / 2f);
            float arcEnd = gapCenter - (currentGapAngleDeg / 2f) + 360f;

            CreateRingWall($"Ring_{i}", radius, arcStart, arcEnd, segmentsPerRing, false);
            Debug.Log($"[MazeGenerator] Created Ring_{i} at radius {radius:F3} with gap at {gapCenter}°");
        }

        // 4. Generate the inner win zone boundary (small circle marking the center target)
        // We must leave a gap so the ball can actually enter the win zone!
        float innerGapCenter = gapAngles[numberOfRings % gapAngles.Length];
        float innerGapAngleDeg = (gapWidth / innerRadius) * Mathf.Rad2Deg;
        
        float innerArcStart = innerGapCenter + (innerGapAngleDeg / 2f);
        float innerArcEnd = innerGapCenter - (innerGapAngleDeg / 2f) + 360f;

        CreateRingWall("InnerBoundary", innerRadius, innerArcStart, innerArcEnd, segmentsPerRing, false);
        Debug.Log($"[MazeGenerator] Created inner boundary with gap at {innerGapCenter}°");

        Debug.Log("[MazeGenerator] Maze generation complete!");
    }

    private void CreateRingWall(string wallName, float radius, float startAngleDeg, float endAngleDeg, int segments, bool closedLoop)
    {
        // Create a child GameObject to hold this wall's EdgeCollider2D
        GameObject wallObj = new GameObject(wallName);
        
        if (mazeContainer != null)
        {
            wallObj.transform.SetParent(mazeContainer, false);
            // Match the OuterRing's scale so the local radius values still map correctly to the sprite size
            wallObj.transform.localScale = transform.localScale;
        }
        else
        {
            wallObj.transform.SetParent(transform, false);
            wallObj.transform.localScale = Vector3.one;
        }
        
        wallObj.transform.localPosition = Vector3.zero;
        wallObj.transform.localRotation = Quaternion.identity;

        EdgeCollider2D edge = wallObj.AddComponent<EdgeCollider2D>();
        // IMPORTANT: edgeRadius is in local space and gets multiplied by the wall's world scale (8.5x).
        // 0.005 local × 8.5 = 0.0425 world units per wall. Two adjacent walls consume 0.085 of the
        // 0.51 channel, leaving 0.425 for the 0.35-diameter ball. Continuous collision detection on the
        // ball's Rigidbody2D prevents tunneling even with this small radius.
        edge.edgeRadius = 0.005f;

        List<Vector2> points = new List<Vector2>();

        // Calculate the total arc angle
        float totalAngle = endAngleDeg - startAngleDeg;
        if (totalAngle < 0f) totalAngle += 360f;
        if (closedLoop) totalAngle = 360f;

        // Generate points along the arc
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angleDeg = startAngleDeg + (totalAngle * t);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;
            points.Add(new Vector2(x, y));
        }

        // For closed loops, connect the last point back to the first
        if (closedLoop)
        {
            points.Add(points[0]);
        }

        edge.points = points.ToArray();

        // Add a visual line so the walls are visible in the Game view
        LineRenderer line = wallObj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        
        // Use the standard 2D sprite shader
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        line.material = lineMat;
        
        // Set the color to black
        line.startColor = Color.black;
        line.endColor = Color.black;
        
        // Make the line thin (remember it gets scaled up by the OuterRing's scale)
        line.startWidth = 0.015f;
        line.endWidth = 0.015f;
        
        // Copy the points from the 2D collider to the 3D line renderer
        line.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, new Vector3(points[i].x, points[i].y, 0f));
        }
    }
}
