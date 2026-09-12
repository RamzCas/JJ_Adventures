using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Diolog_Test : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
    public int backgroundNumber;

    public string[] DialogLines;

    public TextMeshProUGUI characterNameCanvas;
    public string characterName;

    public TextMeshProUGUI TextMeshPro;
    public GameObject Canvas;

    [Header("Player Chat")]
    public GameObject playerCanvas;
    public TextMeshProUGUI playerText;

    public string[] PlayerDialogYap;



    [Header("Ints")]
    public int NumberOfDialog;
    public int CurrentNumberOfDialog;

    public int npcDio = 0;
    public int playerDio = 0;

    [Header("Conversation")]

    public bool InConvo;

    [Header("Other Scripts")]
    public PlayerController PlayerController;
    public QuestManager QuestManager;


    [Header("Quest Settings")]

    public bool giverOfQuest;
    public bool QuestNPC;
    public bool enderOfQuest;

    public string[] questDialog;
    public string[] PlayerQuestYap;



    private PlayerControler Controls;
    private bool wasInConvo;
    /*public int npcDio = 0;
    public int playerDio = 0;*/

    private bool npcTurn = true;
    private static GameObject currentSpeaker;

    private void Awake()
    {
        if (characterNameCanvas != null)
        {
            characterNameCanvas.text = characterName;
        }
        Controls = new PlayerControler();
        RefreshDialogCount();
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

        if (currentSpeaker == gameObject)
        {
            currentSpeaker = null;
        }
    }

    private void Update()
    {

        bool isActiveSpeaker = currentSpeaker == null || currentSpeaker == gameObject;

        if (!InteractPt.activeSelf && isActiveSpeaker)
        {
            InConvo = true;
            currentSpeaker = gameObject;
        }

        if (!isActiveSpeaker)
        {
            return;
        }

        RefreshDialogCount();

        string[] npcLines = GetNpcDialog();
        string[] playerLines = GetPlayerDialog();


        bool npcFinished = npcDio >= npcLines.Length;
        bool playerFinished = playerDio >= playerLines.Length;
        bool conversationFinished = npcFinished && playerFinished;


        if (conversationFinished && enderOfQuest && QuestManager != null && QuestManager.startQuest)
        {
            QuestManager.startQuest = false;
            QuestManager.stopQuest = true;

            Debug.Log("Quest Finished");
        }

        if (conversationFinished && giverOfQuest && QuestManager != null && !QuestManager.startQuest)
        {
            QuestManager.startQuest = true;
            Debug.Log("Start Quest");
        }

        if (conversationFinished)
        {
            InteractPt.SetActive(true);
            InConvo = false;
        }


        if (InConvo && !wasInConvo)
        {
            Canvas.SetActive(true);
            playerCanvas.SetActive(true);
            PlayerController.CanMove = false;
            PlayerController.CurrentSpeed = 0;
            npcTurn = true;

            ShowNextLine();
        }

        else if (!InConvo)
        {
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            playerCanvas.SetActive(false);

            CurrentNumberOfDialog = 0;
            npcDio = 0;
            playerDio = 0;

            npcTurn = true;

            if (currentSpeaker == gameObject)
            {
                currentSpeaker = null;
            }

            PlayerController.CurrentSpeed = PlayerController.Speed;
        }


        wasInConvo = InConvo;
    }


    public void DialogControls(InputAction.CallbackContext context)
    {
        if (!InConvo)
        {
            return;
        }

        if (!context.performed)
        {
            return;
        }


        // Only current NPC can control the conversation.
        if (currentSpeaker != gameObject)
        {
            return;
        }


        //Debug.Log("Convo Progress");


        ShowNextLine();
    }


    private void ShowNextLine()
    {
        // Get current dialogue arrays.
        string[] npcLines = GetNpcDialog();

        string[] playerLines = GetPlayerDialog();


        if (npcTurn)
        {
            if (npcDio < npcLines.Length)
            {
                playerCanvas.SetActive(false);
                Canvas.SetActive(true);

                TextMeshPro.text = npcLines[npcDio];


                //Debug.Log("NPC speaks: Element " + npcDio);

                npcDio++;
                npcTurn = false;
                CurrentNumberOfDialog++;

                return;
            }

            if (playerDio < playerLines.Length)
            {
                npcTurn = false;
                ShowNextLine();
                return;
            }
        }


        else
        {

            if (playerDio < playerLines.Length)
            {
                Canvas.SetActive(false);
                playerCanvas.SetActive(true);
                playerText.text = playerLines[playerDio];


                //Debug.Log("Player speaks: Element " + playerDio);

                playerDio++;
                npcTurn = true;
                CurrentNumberOfDialog++;

                return;
            }

            if (npcDio < npcLines.Length)
            {
                npcTurn = true;

                ShowNextLine();

                return;
            }
        }
    }


    private string[] GetNpcDialog()
    {

        if (QuestNPC &&
            QuestManager != null &&
            QuestManager.startQuest &&
            (giverOfQuest || enderOfQuest))
        {
            if (questDialog != null)
            {
                return questDialog;
            }

            return new string[0];
        }

        if (DialogLines != null)
        {
            return DialogLines;
        }


        return new string[0];
    }


    private string[] GetPlayerDialog()
    {

        if (QuestNPC &&
            QuestManager != null &&
            QuestManager.startQuest &&
            (giverOfQuest || enderOfQuest))
        {
            if (PlayerQuestYap != null)
            {
                return PlayerQuestYap;
            }

            return new string[0];
        }



        if (PlayerDialogYap != null)
        {
            return PlayerDialogYap;
        }


        return new string[0];
    }


    private void RefreshDialogCount()
    {
        string[] npcLines = GetNpcDialog();

        string[] playerLines = GetPlayerDialog();


        NumberOfDialog = npcLines.Length + playerLines.Length;
    }
}


