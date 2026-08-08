using UnityEngine;

public class Reset_Orignal_NPC : MonoBehaviour
{
    public GameObject orinalNPC;
    public GameObject newNPC;


    private void Update()
    {
        if (!orinalNPC.activeSelf) 
        {
            newNPC.SetActive(true);
        }
    }
}
