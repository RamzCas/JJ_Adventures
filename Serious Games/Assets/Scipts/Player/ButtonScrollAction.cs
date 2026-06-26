using System.Collections.Generic;
using UnityEngine;

public class ButtonScrollAction : MonoBehaviour
{
    public GameObject options;
    public float num;

    public float Section;
    public List<GameObject> list;

    private int currentIndex = 0;

    public GameObject[] OtherButtons;

    private void Start()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(i == 0);
        }
    }


    private void Update()
    {
        if(num == 0) 
        {
            options.SetActive(false);
        }


        if(num >= 2) 
        {
            options.SetActive(false);

            foreach (GameObject o in OtherButtons)
            {
                o.SetActive(true);
            }

            num = 0;    
        }


    }


    public void ShowOptions() 
    {
        options.SetActive(true);
        
        num++;

        foreach (GameObject o in OtherButtons) 
        {
            o.SetActive(false);
        }
    }


    public void SectionOptions() 
    {
        Section++;
    }

    public void SectionList() 
    {
        if (list.Count == 0) return;

        // Disable current section
        list[currentIndex].SetActive(false);

        // Cycle to next section
        currentIndex = (currentIndex + 1) % list.Count;

        // Enable next section
        list[currentIndex].SetActive(true);
    }
}
