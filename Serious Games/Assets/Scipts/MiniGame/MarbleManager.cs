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
    private Vector3 dragStartWorldPos;
    private GameObject activeShooter;
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

    private void OnPressStart(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = pointAction.action.ReadValue<Vector2>();

        dragStartWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        dragStartWorldPos.z = 0f;
    }

    private void OnPressRelease(InputAction.CallbackContext ctx)
    {
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
