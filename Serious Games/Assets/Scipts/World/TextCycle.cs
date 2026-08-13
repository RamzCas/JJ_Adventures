using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextCycle : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI TextMeshProUGUI;
    public string[] stringCycle;

    [Header("Lines")]
    public bool continueCycling;
    public int currentLine;
    public int maxLine;
    [Header("Timers")]
    public float timerToNextLine;
    public float maxTime;


    [Header("scenes")]
    public SceneLoader SceneLoader;
    public string marblesSceneName;

    private void Awake()
    {
        maxLine = stringCycle.Length;
        maxLine -= 1;
        continueCycling = true;
    }
    private void Update()
    {
        TextMeshProUGUI.text = stringCycle[currentLine];

        SwitchLines();
        Nextscene();
    }


    public void SwitchLines() 
    {

        timerToNextLine += Time.deltaTime;

        if (continueCycling) 
        {
           
            if (timerToNextLine >= maxTime)
            {
                currentLine += 1;
                timerToNextLine = 0;
            }
        }
        
    }

    public void Nextscene() 
    {
        if(currentLine == maxLine) 
        {
            // ready to switch scene
            continueCycling = false;

            if (timerToNextLine >= maxTime)
            {
                print("Switch Scene");
                SceneManager.LoadScene(marblesSceneName);
            }
        }
    }
}

