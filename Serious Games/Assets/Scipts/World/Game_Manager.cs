using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [Header("Stamps")]
    public int maxStamps;
    public int currentStamps;
    public GameObject finalrig;

    private void Update()
    {
        MoveToMiniGame();
    }


    public void MoveToMiniGame() 
    {
       /* if(currentStamps <= maxStamps) 
        {
            //move to the marble game scene 
        }*/

        if (currentStamps == 8) 
        {
            finalrig.SetActive(true);
        }
    }
}
