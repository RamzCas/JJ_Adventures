using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Diolog_Test : MonoBehaviour
{
    [Header("Components")]
    public GameObject InteractPt;
    public string[] DialogLines;

    public TextMeshProUGUI characterNameCanvas;
    public string characterName;

    public TextMeshProUGUI TextMeshPro;
    public GameObject Canvas;

    [Header("Player Chats")]
    public GameObject playerCanvas;
    public TextMeshProUGUI playerText;
    public string[] PlayerDialogYap;

    [Header("Info")]
    public int NumberOfDialog;       // combined length, informational only
    public int CurrentNumberOfDialog; // total lines shown so far (npc + player)

    [Header("Bools")]
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

    // Independent progress trackers - conversation only ends once both are
    // exhausted, regardless of which array is longer.
    private int npcIndex;
    private int playerIndex;
    private bool npcTurnNext = true;

    // Shared UI (Canvas / playerCanvas) is the same object across every NPC
    // that uses this script. Without this guard, every NPC's Update() fights
    // over that shared object each frame - whichever NPC isn't talking keeps
    // setting it back to inactive, even while another NPC IS talking.
    // This tracks which single NPC currently "owns" the shared dialog UI.
    private static GameObject currentSpeaker;

    private void Awake()
    {
        characterNameCanvas.text = characterName;
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

        // If this NPC gets disabled mid-conversation, release ownership so
        // it doesn't get stuck "holding" the shared UI forever.
        if (currentSpeaker == gameObject)
            currentSpeaker = null;
    }

    private void Update()
    {
        // Not talking, and not the NPC that owns the shared UI right now -
        // don't touch Canvas / playerCanvas / player movement at all.
        bool isActiveSpeaker = currentSpeaker == null || currentSpeaker == gameObject;

        if (!InteractPt.activeSelf && isActiveSpeaker)
        {
            InConvo = true;
            currentSpeaker = gameObject;
        }

        if (!isActiveSpeaker)
        {
            // Someone else is mid-conversation - stay out of the way entirely.
            return;
        }

        RefreshDialogCount();

        var (npcLines, playerLines) = GetActiveDialogArrays();
        bool convoFinished = npcIndex >= npcLines.Length && playerIndex >= playerLines.Length;

        if (convoFinished && enderOfQuest && QuestManager.startQuest)
        {
            QuestManager.startQuest = false;
            QuestManager.stopQuest = true;
        }

        if (convoFinished && giverOfQuest)
        {
            QuestManager.startQuest = true;
            Debug.Log("Start Quest");
        }

        if (convoFinished)
        {
            InteractPt.SetActive(true);
            InConvo = false;
        }

        if (InConvo && !wasInConvo)
        {
            // Just entered conversation this frame - show the opening line.
            Canvas.SetActive(true);
            playerCanvas.SetActive(true);
            PlayerController.CanMove = false;
            PlayerController.CurrentSpeed = 0;
            AdvanceDialog();
        }
        else if (!InConvo)
        {
            PlayerController.CanMove = true;
            Canvas.SetActive(false);
            playerCanvas.SetActive(false);
            CurrentNumberOfDialog = 0;
            npcIndex = 0;
            playerIndex = 0;
            npcTurnNext = true;

            if (currentSpeaker == gameObject)
                currentSpeaker = null;

            PlayerController.CurrentSpeed = PlayerController.Speed;
        }

        wasInConvo = InConvo;
    }

    public void DialogControls(InputAction.CallbackContext context)
    {
        if (InConvo && context.performed && currentSpeaker == gameObject)
        {
            Debug.Log("Convo Progress");
            AdvanceDialog();
        }
    }

    // Shows the next line, alternating npc/player when both still have lines
    // left. If one side has run out, all remaining turns go to whichever
    // side still has lines, so the conversation never ends early - it only
    // stops once BOTH arrays are exhausted.
    private void AdvanceDialog()
    {
        var (npcLines, playerLines) = GetActiveDialogArrays();
        bool npcHasMore = npcIndex < npcLines.Length;
        bool playerHasMore = playerIndex < playerLines.Length;

        if (!npcHasMore && !playerHasMore)
            return;

        bool npcGoesNow = npcHasMore && (npcTurnNext || !playerHasMore);

        if (npcGoesNow)
        {
            TextMeshPro.text = npcLines[npcIndex];
            npcIndex++;
            npcTurnNext = false;
        }
        else if (playerHasMore)
        {
            playerText.text = playerLines[playerIndex];
            playerIndex++;
            npcTurnNext = true;
        }

        CurrentNumberOfDialog++;
    }

    private (string[] npcLines, string[] playerLines) GetActiveDialogArrays()
    {
        bool useQuestDialog = QuestNPC && QuestManager.startQuest && (giverOfQuest || enderOfQuest);
        return useQuestDialog
            ? (questDialog, PlayerQuestYap)
            : (DialogLines, PlayerDialogYap);
    }

    private void RefreshDialogCount()
    {
        var (npcLines, playerLines) = GetActiveDialogArrays();
        NumberOfDialog = npcLines.Length + playerLines.Length;
    }
}