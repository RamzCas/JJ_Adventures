using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AimIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform arrowHead; // Optional Sprite Transform for arrow tip

    [Header("Line Styling")]
    [SerializeField] private float startWidth = 0.15f;
    [SerializeField] private float endWidth = 0.05f;

    [Header("Power Color Settings")]
    [SerializeField] private Color minPowerColor = Color.green;
    [SerializeField] private Color maxPowerColor = Color.red;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Set up LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.useWorldSpace = true;

        HideIndicator();
    }

    public void UpdateIndicator(Vector3 startPoint, Vector3 currentPoint, float powerPercent)
    {
        lineRenderer.enabled = true;

        // Line points: from marble position toward drag direction
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, currentPoint);

        Color currentColor = Color.Lerp(minPowerColor, maxPowerColor, Mathf.Clamp01(powerPercent));

        // Apply color to LineRenderer start and end
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;

        // Update optional arrow tip sprite position & angle
        if (arrowHead != null)
        {
            arrowHead.gameObject.SetActive(true);
            arrowHead.position = currentPoint;
            arrowHead.gameObject.GetComponent<SpriteRenderer>().color = currentColor;

            Vector3 direction = currentPoint - startPoint;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrowHead.rotation = Quaternion.Euler(0, 0, angle); // -90 adjustment depending on sprite orientation
            }
        }
    }

    public void HideIndicator()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (arrowHead != null) arrowHead.gameObject.SetActive(false);
    }
}
