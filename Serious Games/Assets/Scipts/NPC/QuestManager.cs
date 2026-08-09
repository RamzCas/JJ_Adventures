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
    public GameObject onMapQuest;
    public GameObject lineThroughQuest;

    [Header("Lighting")]
    public Lighting Lighting;


    private void Update()
    {

        if (startQuest) 
        {
            icon.SetActive(true);
            onMapQuest.SetActive(true);
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
            lineThroughQuest.SetActive(true);
            //Lighting.currentGlobalLight -= 0.5f;
            //Lighting.currentSpotIntencting += 1.5f;
            stopQuest = false;
        }
    }
}
