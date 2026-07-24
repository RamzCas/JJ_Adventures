using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Quest constrants")]
    public bool startQuest;
    public bool stopQuest;

    [Header("Badges")]
    public Image[] BadgeHolder;
    public int badgeNumber;
    public Sprite[] Stamps;
    public int StampsCount;


    private void Update()
    {
        if (stopQuest) 
        {
            BadgeHolder[badgeNumber].sprite = Stamps[StampsCount];
        }
    }
}
