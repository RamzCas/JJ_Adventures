using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    //Used to load a scene
    public void SwitchScene(string scene)
    {
        //Call this function with the name of the scene (case sensitive)
        SceneManager.LoadSceneAsync(scene);
    }

    public void ReloadScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(sceneName);
    }
    
}
