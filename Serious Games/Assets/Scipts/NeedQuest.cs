using UnityEngine;

public class NeedQuest : MonoBehaviour
{
   public QuestManager QuestManager;
   //public GameObject DoAfterConvo;
   public Do_After_Convo Do_After_Convo;

    private void Update()
    {
        if (QuestManager.startQuest) 
        {
            //DoAfterConvo.SetActive(true);
            Do_After_Convo.enabled = true;
        }
    }
}
