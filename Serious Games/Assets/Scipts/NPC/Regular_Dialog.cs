using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Regular_Dialog : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
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

            InteractPt.SetActive(true);
            InConvo = false;
        }
    }




    private void FixedUpdate()
    {
        if (!InteractPt.activeSelf)
        {
            //Debug.Log("Interated with ncp: Start dilog");
            InConvo = true;

        }


        //chaning the dialog system 
        if (InConvo)
        {
            /*DialogBoards[CurrentNumberOfDialog].gameObject.SetActive(true);
            PlayerController.CanMove = false;
            Canvas.SetActive(true);*/

            Canvas.SetActive(true);
            PlayerController.CanMove = false;

           
            TextMeshPro.text = DialogLines[CurrentNumberOfDialog];
            


        }

        if (!InConvo)
        {
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            CurrentNumberOfDialog = 0;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f)
        {

            //Debug.Log("EndConvo");
            InteractPt.SetActive(true);
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

