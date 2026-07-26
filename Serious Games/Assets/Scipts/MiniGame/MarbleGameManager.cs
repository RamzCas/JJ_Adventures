using System.Collections;
using System.Collections.Generic;
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

    [Header("Game State")]
    public TurnState currentTurn = TurnState.PlayerTurn;
    public int playerScore = 0;
    public int aiScore = 0;

    private List<TargetMarble> targetMarbles = new List<TargetMarble>();
    private GameObject currentShooterMarble;

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
    }

    public void OnShotFired(GameObject shooterInstance)
    {
        currentShooterMarble = shooterInstance;
        StartCoroutine(EvaluateTurnRoutine());
    }

    private IEnumerator EvaluateTurnRoutine()
    {
        currentTurn = TurnState.EvaluatingPhysics;

        // 1. Small buffer delay to allow rigidbodies to register initial launch force
        yield return new WaitForSeconds(0.2f);

        // 2. Wait until ALL marbles (targets + shooter) stop moving
        while (AreMarblesStillMoving())
        {
            yield return null;
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

        // Always clean up the shooter at the end of physics
        /*if (currentShooterMarble != null)
        {
            Destroy(currentShooterMarble);
        }*/

        // 4. Award Score ONLY if the shot was valid (Shooter stayed inside)
        if (!shooterIsOutOfBounds && marblesKnockedOutThisTurn > 0)
        {
            if (previousTurn == TurnState.PlayerTurn)
            {
                playerScore += marblesKnockedOutThisTurn;
            }
            else
            {
                aiScore += marblesKnockedOutThisTurn;
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
            StartTurn(previousTurn); // Repeat turn for same player
        }
        else
        {
            // Pass turn to opponent
            TurnState nextTurn = (previousTurn == TurnState.PlayerTurn) ? TurnState.AITurn : TurnState.PlayerTurn;
            StartTurn(nextTurn);
        }
    }



    private void StartTurn(TurnState turn)
    {
        previousTurn = turn;
        currentTurn = turn;

        if (turn == TurnState.PlayerTurn)
        {
            Debug.Log("--- PLAYER TURN ---");
            // Enable player input here
        }
        else if (turn == TurnState.AITurn)
        {
            Debug.Log("--- AI TURN ---");
            // Trigger AI Shooting logic here
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

}
