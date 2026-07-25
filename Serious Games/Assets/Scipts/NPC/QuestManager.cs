using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Quest constrants")]
    public bool startQuest;
    public bool stopQuest;
    public GameObject giverOfQuestGameObject;
    public Game_Manager gameManager;

    [Header("Badges")]
    public Image[] BadgeHolder;
    public int badgeNumber;
    public Sprite[] Stamps;
    public int StampsCount;

    [Header("Map")]
    public GameObject icon;


    private void Update()
    {

        if (startQuest) 
        {
            icon.SetActive(true);
        }

        if(!startQuest) 
        {
            icon.SetActive(false);
        }

        if (stopQuest) 
        {
            BadgeHolder[badgeNumber].sprite = Stamps[StampsCount];
            giverOfQuestGameObject.SetActive(false);
            gameManager.currentStamps += 1;
            stopQuest = false;
        }
    }
}
