using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{ //Reference to the Character controller
    CharacterController controller;
    float health = 100;
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
    public float gravity = -9.81f; //9.81 m/s²

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;
    public GameObject shield;

    public Animator anim;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        hasShield = false;
    }

    void Update()
    {
        healthSlider.value = health;
        if (inDanger)
        {
            if (!hasShield)
            {
                health -= 7f * Time.deltaTime;
            }
            else 
            {
                health -= .5f * Time.deltaTime;
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


            }
            else 
            {
                moveDirection = Vector3.zero;
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
        if (other.tag == "NPC") 
        {
            if (Input.GetKeyDown(KeyCode.F)) 
            {
            //Start dialogue
            }
        }
        
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Danger")
        {
            inDanger = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(inDanger)inDanger = false;
    }
}
