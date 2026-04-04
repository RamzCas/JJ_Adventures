using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Diolog_Test : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
    public GameObject[] DialogBoards;
    public GameObject Canvas;

    [Header("Array")]
    public int NumberOfDialog;
    public int CurrentNumberOfDialog;

    [Header("Bools")]
    public bool InConvo;

    [Header("Other Scripts")]
    public PlayerController PlayerController;

    PlayerControler Controls;

    private void Awake()
    {
        Controls = new PlayerControler();
        NumberOfDialog = DialogBoards.Length;
    }

    private void OnEnable()
    {
        Controls.Enable();
        Controls.Player.Dialog.performed += DialogControls;
    }

    private void OnDisable()
    {
        Controls.Player.Dialog.performed -= DialogControls;
        Controls.Disable();
    }

    private void FixedUpdate()
    {
        if(!InteractPt.activeSelf) 
        {
            //Debug.Log("Interated with ncp: Start dilog");
            InConvo = true;
            PlayerController.CanMove = false;
            Canvas.SetActive(true); 
        }

        else 
        {
            InConvo = false;
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
        }

        if(InConvo) 
        {
            DialogBoards[CurrentNumberOfDialog].gameObject.SetActive(true);
        }
        
        if(CurrentNumberOfDialog >= NumberOfDialog - 1f) 
        {
            Debug.Log("EndConvo");
            InConvo = false;
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            CurrentNumberOfDialog = 0;
        }
    }



    public void DialogControls(InputAction.CallbackContext context)
    {
        if(InConvo) 
        {
            if (context.performed)
            {
                Debug.Log("Convo");
                CurrentNumberOfDialog++;
            }
        }
     
    }
}
