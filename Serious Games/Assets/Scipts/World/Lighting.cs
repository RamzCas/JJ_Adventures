using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lighting : MonoBehaviour
{
    [Header("Global Lighting")]
    public float minGlobalLight;
    public float currentGlobalLight;
    public Light2D globalLight;

    [Header("Spot lights")]
    public float maxSpotLight;
    public float currentSpotIntencting;
    public Light2D[] spotLight;
    //public GameObject[] spotLightsGameobjects;


    private void Update()
    {
        SetLightIntencity();
    }

    public void SetLightIntencity() 
    {
        globalLight.intensity = currentGlobalLight;

        foreach (Light2D light in spotLight) 
        {
            light.intensity = currentSpotIntencting; 
        }
    }

    public void ManageLightIntencity() 
    {
    
    }
}
