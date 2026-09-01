using UnityEngine;
using UnityEngine.InputSystem;

public class MarbleManager : MonoBehaviour
{
    
    
    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference shootAction;
    [SerializeField]
    private InputActionReference pointAction;

    [Header("Shooting Settings")]
    [SerializeField] 
    private float forceMultiplier = 5f; 
    [SerializeField] 
    private float maxDrag = 3f; 
    [SerializeField] 
    private float minDrag = 0.5f;

    [Header("Misc references")]
    [SerializeField]
    private GameObject shooterMarble;
    [SerializeField] 
    private MarbleGameManager gameManager;
    [SerializeField]
    private AimIndicator aimIndicator;
    private Vector3 dragStartWorldPos;
    private GameObject activeShooter;

    [Header("Shooting rule")]
    [SerializeField]
    private CircleCollider2D ringCollider;
    private float boundaryRadius;
    [SerializeField] 
    private float anchorTouchRadius = 1.2f;
    private bool isValidDragStart;

    private void Start()
    {
        boundaryRadius = ringCollider.radius * ringCollider.transform.localScale.x;
    }
    private void OnEnable()
    {
        shootAction.action.started += OnPressStart;
        shootAction.action.canceled += OnPressRelease;
        shootAction.action.Enable();
        pointAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.started -= OnPressStart;
        shootAction.action.canceled -= OnPressRelease;
        shootAction.action.Disable();
        pointAction.action.Disable();
    }

    private void Update()
    {
        if (!isValidDragStart) return;

        // Read current pointer position in world space
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Vector3 currentDragWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        currentDragWorldPos.z = 0f;

        // Calculate drag vector
        Vector3 dragVector = currentDragWorldPos - dragStartWorldPos;
        float dragDistance = dragVector.magnitude;

        if (dragDistance >= minDrag)
        {
            // Clamp aiming point to maxDrag radius
            Vector3 clampedVector = Vector3.ClampMagnitude(dragVector, maxDrag);
            Vector3 targetAimPoint = dragStartWorldPos + clampedVector;

            // Calculate power ratio between 0.0 (minimum drag) and 1.0 (max drag)
            float powerPercent = clampedVector.magnitude / maxDrag;

            if (aimIndicator != null)
            {
                // Draw line with dynamic color gradient
                aimIndicator.UpdateIndicator(dragStartWorldPos, targetAimPoint, powerPercent);
            }
        }
        else
        {
            // Hide indicator if drag distance is below minDrag threshold
            if (aimIndicator != null)
            {
                aimIndicator.HideIndicator();
            }
        }
    }

    private void OnPressStart(InputAction.CallbackContext ctx)
    {
        if (gameManager.currentTurn != TurnState.PlayerTurn || gameManager.isGameOver)
        {
            return;
        }

        if (gameManager != null && gameManager.currentTurn == TurnState.EvaluatingPhysics) return;
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();

        dragStartWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        dragStartWorldPos.z = 0f;

        if (gameManager != null && gameManager.isAnchored)
        {
            // MUST drag from near the existing shooter marble
            float distanceToMarble = Vector3.Distance(dragStartWorldPos, gameManager.lastShooterPosition);
            if (distanceToMarble <= anchorTouchRadius)
            {
                isValidDragStart = true;
            }
            else
            {
                Debug.Log("Invalid start! Drag from your existing marble to shoot again.");
                isValidDragStart = false;
            }
        }
        else
        {
            // MUST drag from OUTSIDE the boundary ring
            float distanceFromCenter = dragStartWorldPos.magnitude;
            if (distanceFromCenter > boundaryRadius)
            {
                isValidDragStart = true;
            }
            else
            {
                Debug.Log("Invalid start! First shot must start outside the ring.");
                isValidDragStart = false;
            }
        }
    }

    private void OnPressRelease(InputAction.CallbackContext ctx)
    {
        aimIndicator.HideIndicator();
        if (!isValidDragStart) return;
        isValidDragStart = false;

        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();
        Vector3 dragEndWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        dragEndWorldPos.z = 0f;

        Vector3 dragVector = dragEndWorldPos - dragStartWorldPos;
        float dragDistance = dragVector.magnitude;

        if (dragDistance < minDrag) return;

        Vector3 launchVelocity = dragVector.normalized * Mathf.Min(dragDistance, maxDrag) * forceMultiplier;
        LaunchMarble(dragStartWorldPos,launchVelocity);
    }

    private void LaunchMarble(Vector3 spawnPos, Vector3 velocity)
    {
        if (activeShooter != null)
        {
            Destroy(activeShooter);
        }

        activeShooter = Instantiate(shooterMarble,spawnPos,Quaternion.identity);

        if (activeShooter.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = velocity;
        }

        gameManager.OnShotFired(activeShooter);
    }
    
}
