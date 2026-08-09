using UnityEngine;

public class Turn_Off_NPC : MonoBehaviour
{
    public GameObject theTriggr;
    public GameObject NPC;

    private void Update()
    {
        if (!theTriggr.activeSelf) 
        {
            NPC.SetActive(false);
        }
    }
}
