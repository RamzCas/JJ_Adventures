using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MarbleAIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private MarbleGameManager gameManager;
    [SerializeField] 
    private GameObject shooterMarblePrefab;
    [SerializeField] 
    private CircleCollider2D boundaryCollider;

    [Header("AI Settings")]
    [SerializeField] 
    private float thinkDelay = 1.2f;      
    [SerializeField] 
    private float forceMultiplier = 0.2f;    
    [SerializeField] 
    private float maxDrag = 3f;

    [Header("Variance settings")]
    [SerializeField] 
    private float aimErrorDegrees = 3.5f;  
    [SerializeField] 
    private float powerErrorPercent = 0.08f;
    [SerializeField] 
    private float glanceAngleDegrees = 35f;

    private float boundaryRadius;

    private void Start()
    {
        if (boundaryCollider != null)
        {
            boundaryRadius = boundaryCollider.radius * boundaryCollider.transform.localScale.x;
        }
    }

    public void ExecuteAITurn()
    {
        if (gameManager.isGameOver) return;
        StartCoroutine(AITurnRoutine());
    }

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(thinkDelay);

        TargetMarble[] targets = FindObjectsByType<TargetMarble>(FindObjectsSortMode.None);
        if (targets.Length == 0) yield break;

        // 1. Pick a target based on board state
        TargetMarble selectedTarget = PickBestTarget(targets);
        if (selectedTarget == null) yield break;

        // 2. Determine spawn origin based on target
        Vector3 launchOrigin = GetLaunchOrigin(selectedTarget);

        // 3. Aim from origin to target
        Vector3 targetPos = selectedTarget.transform.position;
        Vector3 aimDirection = (targetPos - launchOrigin).normalized;

        float distanceToTarget = Vector3.Distance(launchOrigin, targetPos);
        float distanceTargetToEdge = boundaryRadius - targetPos.magnitude;

        // 4. Force calculation (angled shots need solid power to clip the marble out)
        float pushMultiplier = gameManager.isAnchored ? 0.05f : 0.5f;
        float requiredDistance = distanceToTarget + (distanceTargetToEdge * pushMultiplier);
        float calculatedForce = Mathf.Min(requiredDistance, maxDrag) * forceMultiplier;

        // 5. Apply Inaccuracy
        aimDirection = ApplyAimVariance(aimDirection, aimErrorDegrees);
        calculatedForce *= Random.Range(1f - powerErrorPercent, 1f + powerErrorPercent);

        Vector3 finalVelocity = aimDirection * calculatedForce;

        // 6. Launch!
        LaunchAIShooter(launchOrigin, finalVelocity);
    }

    private Vector3 GetLaunchOrigin(TargetMarble target)
    {
        // If anchored inside, shoot from current marble position
        if (gameManager.isAnchored)
        {
            return gameManager.lastShooterPosition;
        }

        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 exitDirection = targetPos.magnitude > 0.05f ? targetPos.normalized : Vector3.up;

            // Randomize whether to clip from left or right side of the marble
            float sideMultiplier = (Random.value > 0.5f) ? 1f : -1f;

            // Rotate exit vector to create an angled approach
            Quaternion angleOffset = Quaternion.Euler(0f, 0f, glanceAngleDegrees * sideMultiplier);
            Vector3 angledVector = angleOffset * exitDirection;

            float spawnDistance = boundaryRadius + 1.2f;

            // Spawn on the angled side so the AI strikes the marble at a clip!
            return -angledVector * spawnDistance;
        }

        return new Vector3(0f, boundaryRadius + 1.2f, 0f);
    }

    private TargetMarble PickBestTarget(TargetMarble[] targets)
    {
        List<TargetMarble> validTargets = new List<TargetMarble>();

        foreach (TargetMarble target in targets)
        {
            if (target != null && !target.isOutOfBounds) validTargets.Add(target);
        }

        if (validTargets.Count == 0) return null;

        if (gameManager.isAnchored)
        {
            // INSIDE ANCHORED: Pick target closest to current position & edge
            TargetMarble bestTarget = null;
            float minScore = float.MaxValue;

            foreach (var target in validTargets)
            {
                float distFromOrigin = Vector3.Distance(gameManager.lastShooterPosition, target.transform.position);
                float distFromEdge = boundaryRadius - target.transform.position.magnitude;
                float score = distFromOrigin + (distFromEdge * 2f);

                if (score < minScore)
                {
                    minScore = score;
                    bestTarget = target;
                }
            }
            return bestTarget;
        }
        else
        {
            // OUTSIDE SHOT: Filter out dead-center marbles, pick among outer candidates with slight randomness
            validTargets.Sort((a, b) =>
                (boundaryRadius - a.transform.position.magnitude).CompareTo(boundaryRadius - b.transform.position.magnitude));

            // Pick randomly from top 3 closest to edge to avoid repetitive targeting!
            int candidatePool = Mathf.Min(3, validTargets.Count);
            return validTargets[Random.Range(0, candidatePool)];
        }
    }

    private Vector3 ApplyAimVariance(Vector3 originalDirection, float maxDegrees)
    {
        float randomAngle = Random.Range(-maxDegrees, maxDegrees);
        return Quaternion.Euler(0f, 0f, randomAngle) * originalDirection;
    }

    private void LaunchAIShooter(Vector3 spawnPos, Vector3 velocity)
    {
        if (gameManager.isAnchored)
        {
            gameManager.ClearCurrentShooter();
        }

        GameObject aiShooter = Instantiate(shooterMarblePrefab, spawnPos, Quaternion.identity);

        if (aiShooter.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = velocity;
        }

        gameManager.OnShotFired(aiShooter);
    }
}
