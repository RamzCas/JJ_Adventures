using UnityEngine;

public class ShooterMarble : MonoBehaviour
{
    public bool isOutOfBounds = false;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boundary"))
        {
            isOutOfBounds = true;
        }
    }
}
