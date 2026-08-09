using UnityEngine;

public class Trigger_Advice : MonoBehaviour
{
    public string playerTag;
    public GameObject Player; 

    public Trigger_Dialog_Components Trigger_Dialog_Components;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == Player) 
        {
            Trigger_Dialog_Components.InConvo = true;
        }
    }
}
