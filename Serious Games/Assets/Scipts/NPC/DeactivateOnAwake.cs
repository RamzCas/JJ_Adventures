using UnityEngine;

public class DeactivateOnAwake : MonoBehaviour
{
   public Do_After_Convo Do_After_Convo;

    private void Awake()
    {
        Do_After_Convo.enabled = false;
    }
}
