using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trigger_Dialog_Components : MonoBehaviour
{
    [Header("Components")]
    //public GameObject InteractPt;
    //public GameObject[] DialogBoards;//Some reasone we need to add an etra point to work 
    public string[] DialogLines;

    public TextMeshProUGUI TextMeshPro;
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
        //NumberOfDialog = DialogBoards.Length;
        //NumberOfDialog = DialogLines.Length;

        NumberOfDialog = DialogLines.Length;
   

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




    private void Update()
    {
            NumberOfDialog = DialogLines.Length;
 
        if (CurrentNumberOfDialog >= NumberOfDialog - 1f)
        {
        
            InConvo = false;
            this.gameObject.SetActive(false);
        }
    }




    private void FixedUpdate()
    {
        //chaning the dialog system 
        if (InConvo)
        {

            Canvas.SetActive(true);
            //PlayerController.CanMove = false;
            PlayerController.CurrentSpeed = 0;
            TextMeshPro.text = DialogLines[CurrentNumberOfDialog];
        }

        if (!InConvo)
        {
            PlayerController.CanMove = true;
            PlayerController.CurrentSpeed = PlayerController.Speed;
            Canvas.SetActive(false);
            CurrentNumberOfDialog = 0;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f)
        {
            InConvo = false;
        }


    }



    public void DialogControls(InputAction.CallbackContext context)
    {
        if (InConvo)
        {
            if (context.performed)
            {
                Debug.Log("Convo Progress");
                CurrentNumberOfDialog++;
            }
        }

    }
}

