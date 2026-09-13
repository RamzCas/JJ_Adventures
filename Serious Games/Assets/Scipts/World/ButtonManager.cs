using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public string levelScene;

    public GameObject controlsMenu;


   public void Play() 
   {
        SceneManager.LoadScene(levelScene);
   }


    public void Quit() 
    {
        Application.Quit();
    }







    public void OpenControlsMenu() 
    {
        controlsMenu.SetActive(true);
    }

    public void CloseControlsMenu()
    {
        controlsMenu?.SetActive(false);
    }
}
