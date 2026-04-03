using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float Speed;
    public float CurrentSpeed;
   
    public float Rotation;
    public Rigidbody2D rb;
    private Vector2 MoveInput;

    [Header("Sprint")]
    public bool CanSprint;
    public bool IsSprinting;
    public float SprintSpeed;
    public Image SprintImage;
    public GameObject SprintUI;

    [Header("Interaction")]
    public bool CanInteract;
    public float InteractRange;
    public float RayDistance;
    public Transform Player;

    PlayerControler Controls;

    private void Awake()
    {
        Controls = new PlayerControler();
        rb = GetComponent<Rigidbody2D>();
        CurrentSpeed = Speed;
        CanSprint = true;
        //CanInteract = true;
    }

    private void OnEnable()
    {
        Controls.Enable();
        Controls.Player.Interaction.performed += Interact;
        Controls.Player.Sprint.performed += Sprint;
        Controls.Player.Sprint.canceled += Sprint;
    }

    private void OnDisable()
    {
        Controls.Player.Interaction.performed -= Interact;
        Controls.Player.Sprint.performed -= Sprint;
        Controls.Player.Sprint.canceled -= Sprint;
        Controls.Disable();
    }

    private void Update()
    {
        MoveInput = Controls.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = MoveInput * CurrentSpeed;

        if (MoveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(MoveInput.y, MoveInput.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Ray();
        SprintThings();
    }

    public void Ray()
    {
        /*RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.up * RayDistance);
        Debug.DrawRay(transform.position, Vector2.up * RayDistance, Color.yellow);*/

        /* Ray ray = new Ray(Player.position, Player.up);
           RaycastHit hit;

           Debug.DrawRay(Player.position, Player.up * RayDistance, Color.yellow);

           if(Physics.Raycast(ray, out hit, RayDistance)) 
           {
              Debug.Log(hit.collider.gameObject.name);
           }*/

        RaycastHit2D hit2D = Physics2D.Raycast(Player.position, Player.up, RayDistance);
        Debug.DrawRay(Player.position, Player.up * RayDistance, Color.yellow);

        if (hit2D.collider)
        {
            if (hit2D.collider.CompareTag("Inter"))
            {
                //Debug.Log("Interactable");
                CanInteract = true;
            }
        }


        else 
        {
            CanInteract = false;
        }
    }

    public void Interact(InputAction.CallbackContext context) 
    {
        if (CanInteract) 
        {
            if (context.performed) 
            {
                Debug.Log("Interact");
            }
        }
    }

  
    public void Sprint(InputAction.CallbackContext context) 
    {
        if (CanSprint) 
        {
            if (context.performed)
            {
                IsSprinting = true;
                Debug.Log("Sprint");
                CurrentSpeed = SprintSpeed;
            }

            if (context.canceled)
            {
                IsSprinting = false;
                Debug.Log("Walk");
                CurrentSpeed = Speed;
            }
        }
    }


    public void SprintThings() 
    {
        if (IsSprinting) 
        {
            SprintUI.SetActive(true);
            SprintImage.fillAmount -= 0.5f * Time.deltaTime;
        }

        if (!IsSprinting)
        {
            SprintUI.SetActive(false);
            SprintImage.fillAmount += 0.3f * Time.deltaTime;
        }

        if(SprintImage.fillAmount <= 0) 
        {
            StartCoroutine(ReSetSpeed());
        }
    }

    public IEnumerator ReSetSpeed() 
    {
        yield return new WaitForSeconds(0f);
        CanSprint = false;
        IsSprinting = false;
        CurrentSpeed = Speed;
        yield return new WaitForSeconds(2f);
        SprintImage.fillAmount = 1;
        CanSprint = true;
    }
}
