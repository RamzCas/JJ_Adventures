using UnityEngine;

public class TargetMarble : MonoBehaviour
{
    public bool isOutOfBounds = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
