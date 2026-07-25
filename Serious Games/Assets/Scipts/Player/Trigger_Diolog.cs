using UnityEngine;

public class Trigger_Diolog : MonoBehaviour
{
    public string playerTag;
    public GameObject player;
    private BoxCollider2D BoxCollider2D;
    public Trigger_Dialog_Components Trigger_Dialog_Components;

    private void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == player) 
        {
            Trigger_Dialog_Components.InConvo = true;
            //this.gameObject.SetActive(false);
            BoxCollider2D.enabled = false;
        }
    }
}
