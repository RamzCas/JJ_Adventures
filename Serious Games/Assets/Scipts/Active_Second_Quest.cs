using Unity.VisualScripting;
using UnityEngine;

public class Active_Second_Quest : MonoBehaviour
{
    public QuestManager QManager;
    public GameObject secondQuest;


    private void Update()
    {
        if (QManager.stopQuest) 
        { 
            secondQuest.SetActive(true);
        }
    }
}
