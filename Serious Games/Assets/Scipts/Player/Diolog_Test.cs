using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Diolog_Test : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
    //public GameObject[] DialogBoards;//Some reasone we need to add an etra point to work 
    public string[] DialogLines;

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
    /*public Start_Fetch_Qeust Start_Fetch_Quest;
    public FInished_Fetch_Quest Finished_Fetch_Quest;*/

    [Header("Quest Settings")]
    public bool giverOfQuest;
    public bool QuestNPC;
    public bool enderOfQuest;
    public string[] questDialog;


    PlayerControler Controls;

    private void Awake()
    {
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
    }




    private void Update()
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
            InteractPt.SetActive(true);
            //PlayerController.CurrentSpeed = PlayerController.Speed;
            InConvo = false;
        }
    }




    private void FixedUpdate()
    {
        if(!InteractPt.activeSelf) 
        {
            //Debug.Log("Interated with ncp: Start dilog");
            InConvo = true;
            
        }

        
        //chaning the dialog system 
        if(InConvo) 
        {
            /*DialogBoards[CurrentNumberOfDialog].gameObject.SetActive(true);
            PlayerController.CanMove = false;
            Canvas.SetActive(true);*/

            Canvas.SetActive(true);
            PlayerController.CanMove = false;
            PlayerController.CurrentSpeed = 0;

            /*if(QuestManager.startQuest && QuestNPC) 
            {
                TextMeshPro.text = questDialog[CurrentNumberOfDialog];
            }

            
            if(QuestManager.startQuest && giverOfQuest) 
            {
                TextMeshPro.text += questDialog[CurrentNumberOfDialog];
            }

            else
            {
                TextMeshPro.text = DialogLines[CurrentNumberOfDialog];
            }*/

            if (QuestManager.startQuest && enderOfQuest)
            {
                //NumberOfDialog = questDialog.Length;
                TextMeshPro.text = questDialog[CurrentNumberOfDialog];
            }

            if(!QuestManager.startQuest && enderOfQuest) 
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
            //NumberOfDialog = DialogLines.Length;
            PlayerController.CurrentSpeed = PlayerController.Speed;
        }


        if(CurrentNumberOfDialog >= NumberOfDialog - 1f) 
        {
            
            //Debug.Log("EndConvo");
            InteractPt.SetActive(true);
            InConvo = false;
            //PlayerController.CurrentSpeed = PlayerController.Speed;
        }


        if (CurrentNumberOfDialog >= NumberOfDialog - 1f && giverOfQuest)
        {
            QuestManager.startQuest = true;

            Debug.Log("Start Quest");
        }

        /*if (CurrentNumberOfDialog >= NumberOfDialog - 1f && enderOfQuest && QuestManager.startQuest)
        {
            QuestManager.startQuest = false;
            QuestManager.stopQuest = true;
           
            //QuestNPC = false;
        }*/

      
    }



    public void DialogControls(InputAction.CallbackContext context)
    {
        if(InConvo) 
        {
            if (context.performed)
            {
                Debug.Log("Convo Progress");
                CurrentNumberOfDialog++;
            }
        }
     
    }
}
