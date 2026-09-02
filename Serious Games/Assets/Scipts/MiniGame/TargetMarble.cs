using UnityEngine;

public class TargetMarble : MonoBehaviour
{
    public bool isOutOfBounds = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Random.Range(0f, 360f));
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsMoving(float threshold = 0.05f)
    {
        return rb != null && rb.linearVelocity.sqrMagnitude > threshold;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boundary"))
        {
            isOutOfBounds = true;
        }
    }
}
