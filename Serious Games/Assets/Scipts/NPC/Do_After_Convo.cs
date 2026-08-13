using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Do_After_Convo : MonoBehaviour
{
    [Header("Other Scipts")]
    public Diolog_Test Diolog_Test;
    public SceneLoader SceneLoader;

    [Header("State")]

    public WhatToDoAfterConvo afterConvo;
    public enum WhatToDoAfterConvo
    {
        switch_scene,
        exsplainSceneSwitch,
        deactivate,
        activate, 
        nothing,
    }

    [Header("Other Components")]
    public GameObject otherGameObject;
    //public bool switchToMarbelScene;
    public string exsplainerScene;

    private void Update()
    {
        if(Diolog_Test.CurrentNumberOfDialog >= Diolog_Test.NumberOfDialog - 1f) 
        {
            switch (afterConvo) 
            {
                    case WhatToDoAfterConvo.switch_scene:
                    //switch scene
                    break;


                    case WhatToDoAfterConvo.exsplainSceneSwitch:
                    SceneManager.LoadScene(exsplainerScene);
                    break;


                    case WhatToDoAfterConvo.deactivate:
                    otherGameObject.SetActive(false);
                    break;


                    case WhatToDoAfterConvo.activate:
                    otherGameObject.SetActive(true);
                    break;

                    case WhatToDoAfterConvo.nothing:
                    break;
            }
        }

      
    }

}
