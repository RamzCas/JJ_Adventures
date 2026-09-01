using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    AITurn,
    EvaluatingPhysics,
    GameOver
}

public class MarbleGameManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField]
    private Transform marbleContainer;
    [SerializeField] 
    private MarbleManager marbleManager;
    private TurnState previousTurn = TurnState.PlayerTurn;
    [SerializeField] 
    private MarbleAIController aiController;
    private SceneLoader sceneLoader;

    [Header("Game State")]
    public TurnState currentTurn = TurnState.PlayerTurn;
    public int playerScore = 0;
    public int aiScore = 0;

    [Header("UI Text")]
    [SerializeField]
    private TextMeshProUGUI playerText;
    [SerializeField]
    private TextMeshProUGUI oppText;
    [SerializeField] 
    private GameObject winPanel;
    [SerializeField] 
    private GameObject losePanel;

    [Header("Shooting Rule")]
    public bool isAnchored = false;
    public Vector3 lastShooterPosition;

    private List<TargetMarble> targetMarbles = new List<TargetMarble>();
    private GameObject currentShooterMarble;
    [SerializeField]
    private GameObject playerTurnPanel;
    [SerializeField]
    private GameObject oppTurnPanel;
    public bool isGameOver = false;


    private void Awake()
    {
        if (marbleContainer != null)
        {
            targetMarbles.AddRange(marbleContainer.GetComponentsInChildren<TargetMarble>());
        }
    }

    private void Start()
    {
        StartTurn(TurnState.PlayerTurn);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    public void OnShotFired(GameObject shooterInstance)
    {
        currentShooterMarble = shooterInstance;
        StartCoroutine(EvaluateTurnRoutine());
    }

    private IEnumerator EvaluateTurnRoutine()
    {
        currentTurn = TurnState.EvaluatingPhysics;

        playerTurnPanel.SetActive(false);
        oppTurnPanel.SetActive(false);

        // 1. Small buffer delay to allow rigidbodies to register initial launch force
        yield return new WaitForSeconds(0.2f);

        // 2. Wait until ALL marbles (targets + shooter) stop moving
        while (AreMarblesStillMoving())
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (AreMarblesStillMoving())
        {
            yield return null;
        }

        if (CheckGameOver())
        {
            // Stop turn cycling since the game has ended!
            yield break;
        }
        // 3. Evaluate results
        int marblesKnockedOutThisTurn = 0;
        bool shooterIsOutOfBounds = IsShooterOutOfBounds();

        // Remove and destroy ANY target marble that went out of bounds
        for (int i = targetMarbles.Count - 1; i >= 0; i--)
        {
            TargetMarble marble = targetMarbles[i];
            if (marble.isOutOfBounds)
            {
                marblesKnockedOutThisTurn++;
                targetMarbles.RemoveAt(i);
                Destroy(marble.gameObject); // Always clean them off the board!
            }
        }

        // 4. Award Score if the shot was valid 
        if (!shooterIsOutOfBounds && marblesKnockedOutThisTurn > 0)
        {
            if (previousTurn == TurnState.PlayerTurn)
            {
                playerScore += marblesKnockedOutThisTurn;
                playerText.text = $"Player Score: {playerScore}";

               
            }
            else
            {
                aiScore += marblesKnockedOutThisTurn;
                oppText.text = $"Opponent Score: {aiScore}";
            }
            Debug.Log($"Valid Shot! Player Score: {playerScore} | AI Score: {aiScore}");
        }
        else if (shooterIsOutOfBounds && marblesKnockedOutThisTurn > 0)
        {
            Debug.Log("Foul! Target knocked out, but shooter left the boundary. No points awarded.");
        }

        // 5. Check Win Condition
        if (targetMarbles.Count == 0)
        {
            EndGame();
            yield break;
        }

        // 6. Determine Next Turn
        // Legal hit = Knocked out >= 1 marble AND shooter stayed inside
        bool keepsTurn = (marblesKnockedOutThisTurn > 0) && !shooterIsOutOfBounds;

        if (keepsTurn)
        {
            isAnchored = true;
            if (currentShooterMarble != null)
            {
                lastShooterPosition = currentShooterMarble.transform.position;
            }
            StartTurn(previousTurn); // Repeat turn for same player
        }
        else
        {
            if (currentShooterMarble != null)
            {
                Destroy(currentShooterMarble);
            }
            isAnchored = false;
            // Pass turn to opponent
            TurnState nextTurn = (previousTurn == TurnState.PlayerTurn) ? TurnState.AITurn : TurnState.PlayerTurn;
            StartTurn(nextTurn);
        }
    }



    private void StartTurn(TurnState turn)
    {
        previousTurn = turn;
        currentTurn = turn;

        playerTurnPanel.SetActive(turn == TurnState.PlayerTurn);
        oppTurnPanel.SetActive(turn == TurnState.AITurn);

        if (turn == TurnState.PlayerTurn)
        {
            Debug.Log("--- PLAYER TURN ---");
        }
        else if (turn == TurnState.AITurn)
        {
            Debug.Log("--- AI TURN ---");
            aiController.ExecuteAITurn();
        }
    }

    private bool AreMarblesStillMoving()
    {
        // Check target marbles
        foreach (var marble in targetMarbles)
        {
            if (marble != null && marble.IsMoving())
                return true;
        }

        // Check shooter marble
        if (currentShooterMarble != null)
        {
            if (currentShooterMarble.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                if (rb.linearVelocity.sqrMagnitude > 0.05f)
                    return true;
            }
        }

        return false;
    }

    private bool IsShooterOutOfBounds()
    {
        if (currentShooterMarble == null) return false;

        if (currentShooterMarble.TryGetComponent<ShooterMarble>(out ShooterMarble boundaryChecker))
        {
            return boundaryChecker.isOutOfBounds;
        }

        return false;
    }

    private void EndGame()
    {
        currentTurn = TurnState.GameOver;
        Debug.Log($"Game Over! Final Score -> Player: {playerScore} | AI: {aiScore}");
    }

    public void ClearCurrentShooter()
    {
        if (currentShooterMarble != null)
        {
            Destroy(currentShooterMarble);
            currentShooterMarble = null;
        }
    }

    private bool CheckGameOver()
    {
        // Find remaining active target marbles
        TargetMarble[] activeTargets = FindObjectsByType<TargetMarble>(FindObjectsSortMode.None);

        int remainingInRing = 0;
        foreach (var target in activeTargets)
        {
            // Only count target marbles that are valid and still inside the boundary
            if (target != null && !target.isOutOfBounds)
            {
                remainingInRing++;
            }
        }

        // Game ends when no target marbles remain in the ring
        if (remainingInRing == 0)
        {
            isGameOver = true;

            if (playerScore > aiScore)
            {
                if (winPanel != null) winPanel.SetActive(true);
                StartCoroutine(FinishGame());
            }
            else
            {
                StartCoroutine(RestartGame());
                if (losePanel != null) losePanel.SetActive(true);
            }

            if (playerTurnPanel != null) playerTurnPanel.SetActive(false);
            if (oppTurnPanel != null) oppTurnPanel.SetActive(false);

            return true;
        }

        return false;
    }

    private IEnumerator RestartGame()
    {
        Debug.Log("Switch scenes please");
        yield return new WaitForSecondsRealtime(3);
        sceneLoader.ReloadScene();
    }

    private IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(3);
        sceneLoader.SwitchScene("GameEnd");
    }

}
