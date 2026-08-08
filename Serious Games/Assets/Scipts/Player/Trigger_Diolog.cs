using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trigger_Diolog : MonoBehaviour
{
    public string playerTag;
    public GameObject player;
    //private BoxCollider2D BoxCollider2D;
    public Diolog_Test Diolog_Test;


    /*public string[] DialogLines;

    public TextMeshProUGUI characterNameCanvas;
    public string characterName;

    public TextMeshProUGUI TextMeshPro;
    public GameObject Canvas;



    [Header("Array")]
    public int NumberOfDialog;
    public int CurrentNumberOfDialog;

    [Header("Bools")]
    public bool InConvo;

    [Header("Other Scripts")]
    public PlayerController PlayerController;
    public QuestManager QuestManager;
    *//*public Start_Fetch_Qeust Start_Fetch_Quest;
    public FInished_Fetch_Quest Finished_Fetch_Quest;*//*

    [Header("Quest Settings")]
    public bool giverOfQuest;
    public bool QuestNPC;
    public bool enderOfQuest;
    public string[] questDialog;

    PlayerControler Controls;

    private void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        characterNameCanvas.text = characterName;
        Controls = new PlayerControler();
        //NumberOfDialog = DialogBoards.Length;
        //NumberOfDialog = DialogLines.Length;

        if (QuestNPC)
        {
            if (QuestManager.startQuest)
            {
                NumberOfDialog = questDialog.Length;
            }

            else
            {
                NumberOfDialog = DialogLines.Length;
            }
        }

        else
        {
            NumberOfDialog = DialogLines.Length;
        }
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
    } */



    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == player) 
        {
            //Trigger_Dialog_Components.InConvo = true;
            //this.gameObject.SetActive(false);
            //BoxCollider2D.enabled = false;
            Diolog_Test.InConvo = true;
            this.gameObject.SetActive(false);
        }
    }

   /* private void Update()
    {
        if (QuestManager.startQuest && enderOfQuest)
        {
            NumberOfDialog = questDialog.Length;
        }

        if (!QuestManager.startQuest && enderOfQuest)
        {
            NumberOfDialog = DialogLines.Length;
        }

        if (QuestManager.startQuest && giverOfQuest)
        {
            NumberOfDialog = questDialog.Length;
        }

        if (!QuestManager.startQuest && giverOfQuest)
        {
            NumberOfDialog = DialogLines.Length;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f && enderOfQuest && QuestManager.startQuest)
        {
            QuestManager.startQuest = false;
            QuestManager.stopQuest = true;
            InConvo = false;
        }
    }

    private void FixedUpdate()
    {
        if (InConvo)
        {

            Canvas.SetActive(true);
            PlayerController.CanMove = false;

            if (QuestManager.startQuest && enderOfQuest)
            {
                //NumberOfDialog = questDialog.Length;
                TextMeshPro.text = questDialog[CurrentNumberOfDialog];
            }

            if (!QuestManager.startQuest && enderOfQuest)
            {
                TextMeshPro.text = DialogLines[CurrentNumberOfDialog];
            }

            if (QuestManager.startQuest && giverOfQuest)
            {
                //NumberOfDialog = questDialog.Length;
                TextMeshPro.text = questDialog[CurrentNumberOfDialog];
            }

            if (!QuestManager.startQuest && giverOfQuest)
            {
                //NumberOfDialog = DialogLines.Length;
                TextMeshPro.text = DialogLines[CurrentNumberOfDialog];
            }

        }

        if (!InConvo)
        {
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            CurrentNumberOfDialog = 0;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f)
        {
            InConvo = false;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f && giverOfQuest)
        {
            QuestManager.startQuest = true;

            Debug.Log("Start Quest");
        }


    }










    public void DialogControls(InputAction.CallbackContext context)
    {
        if (InConvo)
        {
            if (context.performed)
            {
                //Debug.Log("Convo Progress");
                CurrentNumberOfDialog++;
            }
        }

    }*/
}
