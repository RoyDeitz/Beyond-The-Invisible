using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuantumTek.QuantumDialogue.Demo;

public class PlayerController : MonoBehaviour
{ //Reference to the Character controller
    CharacterController controller;
    public float health = 100;
    public float minDamage = .5f;
    public float maxDamage = 7f;
    public Slider healthSlider;
    bool hasShield=false;
    bool inDanger = false;

    //Player Movement Speed
    public float movementSpeed = 12f;
    public float sprintFactor = 2f;
    public float groundCheckRadius = .1f;
    Vector3 movementVector;
    Vector3 moveDirection;
    public float rotationSmoothTime = .1f;
    float rotationSmoothVelocity;
    public Transform cam;

    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s?

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;
    public GameObject shield;

    public Animator anim;

    public bool isSpeaking = false;
    public Canvas canvasDialogue;
    public Canvas canvasStartConversation;
    public Canvas canvasStartInspect;
    public AudioSource footSound;

    void Start()
    {
        canvasStartConversation.enabled = false; 
        canvasDialogue.enabled=false;
        canvasStartInspect.enabled=false;
        isSpeaking = false;
        controller = gameObject.GetComponent<CharacterController>();
        hasShield = false;
        footSound.enabled = false;
    }

    void Update()
    {
        healthSlider.value = health;
        if (inDanger)
        {
            if (!hasShield)
            {
                health -= maxDamage * Time.deltaTime;
            }
            else 
            {
                health -= minDamage* Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.V)) 
        {
            hasShield = !hasShield;
        }
        if (hasShield)
        {
            shield.SetActive(true);
        }
        else 
        {
            shield.SetActive(false);
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("Grounded", isGrounded);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //Only change the movement Vector if grounded
        if (isGrounded)
        {
            //movementVector = transform.forward * z + transform.right * x;
            movementVector = new Vector3(x, 0, z);

            //if (movementVector.magnitude > 1f)
            //    movementVector = movementVector.normalized;

            if (movementVector.magnitude > .1f)
            {
                movementVector = movementVector.normalized;
                float targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                footSound.enabled = true;


            }
            else 
            {
                moveDirection = Vector3.zero;
                footSound.enabled = false;
            }
        }



        //Double the speed on sprint button
        if (Input.GetKey(KeyCode.LeftShift) && z > 0 && isGrounded == true)
        {
            movementVector = movementVector * sprintFactor;
        }

        anim.SetFloat("Speed", (moveDirection.normalized * movementSpeed).magnitude);
        controller.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);

        //Jump
        /*
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        */
        //Reset the vertical velocity to avoid it being to strong
        if (isGrounded == true && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }
        else
        {
            //Apply gravity
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            if (!isSpeaking)
            {
                canvasStartConversation.enabled=true;
                Debug.Log("enter trigger");
            }
        }

      /* if (other.gameObject.tag == "ParticleDanger")
        {
            
            if (hasShield)
            {
                health -= minDamage * .5f;
                Destroy(other.gameObject);
            }
            else
            {
                health -= maxDamage * .4f;
                Destroy(other.gameObject);
            }

        }*/
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Danger" )
        {
            inDanger = true;

        }
        if (other.gameObject.tag == "NPC")
        {
            if (!isSpeaking)
            {

                if (Input.GetKeyDown(KeyCode.F))
                {
                    isSpeaking = true;
                    canvasStartConversation.enabled = false;
                    Debug.Log("pressing button f");
                    if (other.gameObject.GetComponent<NPC>() != null)
                    {
                        canvasDialogue.GetComponent<QD_DialogueDemo>().conversationTitle = other.gameObject.GetComponent<NPC>().conversationTitle;
                        canvasDialogue.enabled = true;
                        Debug.Log("conversation started");
                    }
                    else 
                    {
                        Debug.Log("conversation not Found");
                    }
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(inDanger)inDanger = false;
       
        if (canvasStartConversation.enabled==true)canvasStartConversation.enabled = false;

        Debug.Log("exit Trigger");
    }
}
