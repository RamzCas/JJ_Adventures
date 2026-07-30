using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Regular_Dialog : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
    //public GameObject[] DialogBoards;//Some reasone we need to add an etra point to work 
    public string[] DialogLines;

    public string[] playerDialog;

    public TextMeshProUGUI TextMeshPro;
    public GameObject Canvas;

    public TextMeshProUGUI playerTextMeshPro;
    public GameObject playerCanvas;

    [Header("Array")]
    public int NumberOfDialog;
    public int CurrentNumberOfDialog;
    public int playerNumberOfDialog;

    [Header("Bools")]
    public bool InConvo;

    [Header("Other Scripts")]
    public PlayerController PlayerController;


    PlayerControler Controls;

    private void Awake()
    {
        Controls = new PlayerControler();

        NumberOfDialog = DialogLines.Length;
        //playerNumberOfDialog -= 1;
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
            InConvo = true;
        }

        if (InConvo)
        {
            Canvas.SetActive(true);
            PlayerController.CanMove = false;

            TextMeshPro.text = DialogLines[CurrentNumberOfDialog];

            // Player response lags one index behind the NPC line
            playerNumberOfDialog = CurrentNumberOfDialog - 1;

            if (playerNumberOfDialog >= 0 && playerNumberOfDialog < playerDialog.Length)
            {
                playerCanvas.SetActive(true);
                playerTextMeshPro.text = playerDialog[playerNumberOfDialog];
            }
            else
            {
                // no player line yet (first NPC line has no response before it)
                playerCanvas.SetActive(false);
            }
        }

        if (!InConvo)
        {
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            playerCanvas.SetActive(false);
            CurrentNumberOfDialog = 0;
        }

        if (playerNumberOfDialog >= NumberOfDialog - 1f)
        {
            InteractPt.SetActive(true);
            InConvo = false;
        }

        if (CurrentNumberOfDialog >= NumberOfDialog - 1f)
        {
            //Canvas.SetActive(false);
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
                playerNumberOfDialog ++;
               
            }
        }

    }
}

