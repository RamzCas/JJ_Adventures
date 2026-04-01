using UnityEngine;

public class RayTest : MonoBehaviour
{
    [Header("Interaction")]
    public float RayDistance;
    public Transform Player;

    void Update()
    {
        
    }

    public void Ray()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Player.position, Vector2.up * RayDistance);
        Debug.DrawRay(Player.position, Vector2.up * RayDistance, Color.yellow);
    }
}
