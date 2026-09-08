using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Diolog_Test : MonoBehaviour
{
    // =========================================================
    // COMPONENTS
    // =========================================================

    [Header("Components")]

    public GameObject InteractPt;

    // Normal NPC dialogue.
    public string[] DialogLines;

    public TextMeshProUGUI characterNameCanvas;
    public string characterName;

    // NPC dialogue text.
    public TextMeshProUGUI TextMeshPro;

    public GameObject Canvas;


    // =========================================================
    // PLAYER CHATS
    // =========================================================

    [Header("Player Chats")]

    public GameObject playerCanvas;

    // Player dialogue text.
    public TextMeshProUGUI playerText;

    // Normal player dialogue.
    //
    // DialogLines[0] goes with PlayerDialogYap[0]
    // DialogLines[1] goes with PlayerDialogYap[1]
    // etc.
    public string[] PlayerDialogYap;


    // =========================================================
    // INFO
    // =========================================================

    [Header("Info")]

    // Total number of lines.
    //
    // Example:
    // 3 NPC lines + 3 Player lines = 6
    public int NumberOfDialog;

    // Number of lines that have already been shown.
    public int CurrentNumberOfDialog;


    // =========================================================
    // CONVERSATION
    // =========================================================

    [Header("Conversation")]

    public bool InConvo;


    // =========================================================
    // OTHER SCRIPTS
    // =========================================================

    [Header("Other Scripts")]

    public PlayerController PlayerController;

    public QuestManager QuestManager;


    // =========================================================
    // QUEST SETTINGS
    // =========================================================

    [Header("Quest Settings")]

    public bool giverOfQuest;

    public bool QuestNPC;

    public bool enderOfQuest;

    // Quest NPC dialogue.
    public string[] questDialog;

    // Quest Player dialogue.
    public string[] PlayerQuestYap;


    // =========================================================
    // PRIVATE VARIABLES
    // =========================================================

    private PlayerControler Controls;

    private bool wasInConvo;


    // Current NPC dialogue element.
    private int npcDio = 0;

    // Current Player dialogue element.
    private int playerDio = 0;


    // =========================================================
    // TURN
    // =========================================================
    //
    // true  = NPC speaks next
    // false = Player speaks next
    //
    // NPC ALWAYS starts the conversation.
    private bool npcTurn = true;


    // =========================================================
    // SHARED UI OWNER
    // =========================================================
    //
    // This prevents multiple NPCs from controlling the same
    // Canvas at the same time.
    private static GameObject currentSpeaker;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // Set NPC name.
        if (characterNameCanvas != null)
        {
            characterNameCanvas.text = characterName;
        }

        // Create Input System.
        Controls = new PlayerControler();

        // Calculate dialogue count.
        RefreshDialogCount();
    }


    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        Controls.Enable();

        Controls.Player.Dialog.performed += DialogControls;
    }


    // =========================================================
    // ON DISABLE
    // =========================================================

    private void OnDisable()
    {
        Controls.Player.Dialog.performed -= DialogControls;

        Controls.Disable();

        // Release shared UI.
        if (currentSpeaker == gameObject)
        {
            currentSpeaker = null;
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // -----------------------------------------------------
        // CHECK WHO OWNS THE DIALOGUE UI
        // -----------------------------------------------------

        bool isActiveSpeaker =
            currentSpeaker == null ||
            currentSpeaker == gameObject;


        // -----------------------------------------------------
        // START CONVERSATION
        // -----------------------------------------------------

        if (!InteractPt.activeSelf && isActiveSpeaker)
        {
            InConvo = true;

            currentSpeaker = gameObject;
        }


        // Another NPC is currently talking.
        if (!isActiveSpeaker)
        {
            return;
        }


        // -----------------------------------------------------
        // UPDATE NUMBER OF DIALOGUE LINES
        // -----------------------------------------------------

        RefreshDialogCount();


        // -----------------------------------------------------
        // CHECK IF CONVERSATION IS FINISHED
        // -----------------------------------------------------

        string[] npcLines = GetNpcDialog();
        string[] playerLines = GetPlayerDialog();


        bool npcFinished =
            npcDio >= npcLines.Length;

        bool playerFinished =
            playerDio >= playerLines.Length;


        bool conversationFinished =
            npcFinished && playerFinished;


        // -----------------------------------------------------
        // QUEST ENDER
        // -----------------------------------------------------

        if (conversationFinished &&
            enderOfQuest &&
            QuestManager != null &&
            QuestManager.startQuest)
        {
            QuestManager.startQuest = false;

            QuestManager.stopQuest = true;

            Debug.Log("Quest Finished");
        }


        // -----------------------------------------------------
        // QUEST GIVER
        // -----------------------------------------------------

        if (conversationFinished &&
            giverOfQuest &&
            QuestManager != null &&
            !QuestManager.startQuest)
        {
            QuestManager.startQuest = true;

            Debug.Log("Start Quest");
        }


        // -----------------------------------------------------
        // END CONVERSATION
        // -----------------------------------------------------

        if (conversationFinished)
        {
            InteractPt.SetActive(true);

            InConvo = false;
        }


        // -----------------------------------------------------
        // JUST ENTERED CONVERSATION
        // -----------------------------------------------------

        if (InConvo && !wasInConvo)
        {
            // Show dialogue UI.
            Canvas.SetActive(true);

            playerCanvas.SetActive(true);


            // Stop player movement.
            PlayerController.CanMove = false;

            PlayerController.CurrentSpeed = 0;


            // NPC ALWAYS speaks first.
            npcTurn = true;

            ShowNextLine();
        }


        // -----------------------------------------------------
        // NOT IN CONVERSATION
        // -----------------------------------------------------

        else if (!InConvo)
        {
            // Allow player movement.
            PlayerController.CanMove = true;


            // Hide dialogue UI.
            Canvas.SetActive(false);

            playerCanvas.SetActive(false);


            // Reset dialogue.
            CurrentNumberOfDialog = 0;

            npcDio = 0;

            playerDio = 0;


            // NPC starts next conversation.
            npcTurn = true;


            // Release Canvas.
            if (currentSpeaker == gameObject)
            {
                currentSpeaker = null;
            }


            // Restore player speed.
            PlayerController.CurrentSpeed =
                PlayerController.Speed;
        }


        wasInConvo = InConvo;
    }


    // =========================================================
    // DIALOGUE INPUT
    // =========================================================

    public void DialogControls(InputAction.CallbackContext context)
    {
        // Must be talking.
        if (!InConvo)
        {
            return;
        }


        // Only react when button is actually pressed.
        if (!context.performed)
        {
            return;
        }


        // Only current NPC can control the conversation.
        if (currentSpeaker != gameObject)
        {
            return;
        }


        Debug.Log("Convo Progress");


        // Show exactly ONE new line.
        ShowNextLine();
    }


    // =========================================================
    // SHOW NEXT LINE
    // =========================================================

    private void ShowNextLine()
    {
        // Get current dialogue arrays.
        string[] npcLines = GetNpcDialog();

        string[] playerLines = GetPlayerDialog();


        // =====================================================
        // NPC SPEAKS
        // =====================================================

        if (npcTurn)
        {
            // Make sure NPC has another line.
            if (npcDio < npcLines.Length)
            {
                // Hide player dialogue.
                playerCanvas.SetActive(false);

                // Show NPC dialogue.
                Canvas.SetActive(true);

                TextMeshPro.text =
                    npcLines[npcDio];


                Debug.Log(
                    "NPC speaks: Element " +
                    npcDio
                );


                // Move to next NPC element.
                npcDio++;


                // Player speaks next.
                npcTurn = false;


                // Count this line.
                CurrentNumberOfDialog++;

                return;
            }


            // NPC has no more lines.
            //
            // If player still has lines, let player speak.
            if (playerDio < playerLines.Length)
            {
                npcTurn = false;

                ShowNextLine();

                return;
            }
        }


        // =====================================================
        // PLAYER SPEAKS
        // =====================================================

        else
        {
            // Make sure player has another line.
            if (playerDio < playerLines.Length)
            {
                // Hide NPC dialogue.
                Canvas.SetActive(false);

                // Show player dialogue.
                playerCanvas.SetActive(true);

                playerText.text =
                    playerLines[playerDio];


                Debug.Log(
                    "Player speaks: Element " +
                    playerDio
                );


                // Move to next Player element.
                playerDio++;


                // NPC speaks next.
                npcTurn = true;


                // Count this line.
                CurrentNumberOfDialog++;

                return;
            }


            // Player has no more lines.
            //
            // If NPC still has lines, let NPC speak.
            if (npcDio < npcLines.Length)
            {
                npcTurn = true;

                ShowNextLine();

                return;
            }
        }
    }


    // =========================================================
    // GET NPC DIALOGUE
    // =========================================================

    private string[] GetNpcDialog()
    {
        // -----------------------------------------------------
        // QUEST DIALOGUE
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // NORMAL DIALOGUE
        // -----------------------------------------------------

        if (DialogLines != null)
        {
            return DialogLines;
        }


        return new string[0];
    }


    // =========================================================
    // GET PLAYER DIALOGUE
    // =========================================================

    private string[] GetPlayerDialog()
    {
        // -----------------------------------------------------
        // QUEST PLAYER DIALOGUE
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // NORMAL PLAYER DIALOGUE
        // -----------------------------------------------------

        if (PlayerDialogYap != null)
        {
            return PlayerDialogYap;
        }


        return new string[0];
    }


    // =========================================================
    // REFRESH DIALOGUE COUNT
    // =========================================================

    private void RefreshDialogCount()
    {
        string[] npcLines = GetNpcDialog();

        string[] playerLines = GetPlayerDialog();


        NumberOfDialog =
            npcLines.Length +
            playerLines.Length;
    }
}
