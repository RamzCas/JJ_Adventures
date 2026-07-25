using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [Header("Stamps")]
    public int maxStamps;
    public int currentStamps;


    private void Update()
    {
        MoveToMiniGame();
    }




    public void MoveToMiniGame() 
    {
        if(currentStamps <= maxStamps) 
        {
            //move to the marble game scene 
        }
    }
}
