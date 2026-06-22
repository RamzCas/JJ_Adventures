using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float Speed;
    public float CurrentSpeed;
    public bool CanMove;
   
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

    [Header("UI Guide")]
    public int UI_Count;
    public GameObject UI_Base;
   /* public GameObject PlayerCam;
    public GameObject OverviewCam;*/


    PlayerControler Controls;

    private void Awake()
    {
        Controls = new PlayerControler();
        rb = GetComponent<Rigidbody2D>();
        CurrentSpeed = Speed;
        CanSprint = true;
        CanMove = true;
        //CanInteract = true;
    }

    private void OnEnable()
    {
        Controls.Enable();
        Controls.Player.Interaction.performed += Interact;
        Controls.Player.Sprint.performed += Sprint;
        Controls.Player.Sprint.canceled += Sprint;
        Controls.Player.Map.performed += Map_UI;
        //Controls.Player.Dialog.performed += DialogControls;
        //Controls.Player.Dialog.canceled += DialogControls;
    }

    private void OnDisable()
    {
        Controls.Player.Interaction.performed -= Interact;
        Controls.Player.Sprint.performed -= Sprint;
        Controls.Player.Sprint.canceled -= Sprint;
        Controls.Player.Map.canceled -= Map_UI;
        //Controls.Player.Dialog.performed -= DialogControls;
        //Controls.Player.Dialog.canceled -= DialogControls;


        Controls.Disable();
    }

    private void Update()
    {
        MoveInput = Controls.Player.Movement.ReadValue<Vector2>();
        MapUI_Manager();
    }

    private void FixedUpdate()
    {
        if (CanMove) 
        {
            rb.linearVelocity = MoveInput * CurrentSpeed;

            if (MoveInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(MoveInput.y, MoveInput.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

          
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
                Debug.Log("Interactable");
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
                RaycastHit2D hit2D = Physics2D.Raycast(Player.position, Player.up, RayDistance);
                Debug.DrawRay(Player.position, Player.up * RayDistance, Color.blueViolet);

                if (hit2D.collider)
                {
                    if (hit2D.collider.CompareTag("Inter"))
                    {
                        Debug.Log("Interactable");
                        hit2D.collider.gameObject.SetActive(false);
                    }
                }
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

    public void Map_UI(InputAction.CallbackContext context) 
    {
        print("open Map");
        UI_Count++;
    }

    public void MapUI_Manager() 
    {
     if(UI_Count == 1) 
        {
            UI_Base.SetActive(true);
            //CanMove = false;
            CurrentSpeed = 0;
            /*PlayerCam.SetActive(false);
            OverviewCam.SetActive(true);*/
        }

     if (UI_Count >= 2) 
        {
            UI_Base.SetActive(false);
            // CanMove = true;
            CurrentSpeed = Speed;
            UI_Count = 0;
       /*     PlayerCam.SetActive(true);
            OverviewCam.SetActive(false);*/
        }
    }
  
}
