using UnityEngine;

public class Activate_NPC : MonoBehaviour
{
    public GameObject theTriggr;
    public GameObject NPC;

    private void Update()
    {
        if (theTriggr.activeSelf)
        {
            NPC.SetActive(true);
        }
    }
}
