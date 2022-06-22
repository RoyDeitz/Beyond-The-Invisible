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
    Vector3 movementVector;

    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s²

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;
    public GameObject shield;

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
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundLayer);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Only change the movement Vector if grounded
        if (isGrounded)
        {
            movementVector = transform.forward * z + transform.right * x;
            if (movementVector.magnitude > 1f)
                movementVector = movementVector.normalized;
        }

        //Double the speed on sprint button
        if (Input.GetKey(KeyCode.LeftShift) && z > 0 && isGrounded == true) movementVector = movementVector * sprintFactor;

        controller.Move(movementVector * movementSpeed * Time.deltaTime);

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

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
