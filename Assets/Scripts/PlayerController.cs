using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables
    public float turnSmoothing = 15f; // A smoothing value for turning the player
    public float jumpSpeed = 1f; // Player jump speed
    public GameObject grenadeObject;
    public float grenadeTime = 3f;
    public float throwAngle = 45f;
    public float throwGravity = 9.8f;

    private Animator m_Animator; // Reference to the animator component
    private Rigidbody m_Rigidbody;
    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private Quaternion m_TargetRotation = Quaternion.identity;
    private Vector3 m_ThrowDirection;
    private Collider m_Collider;
    private Camera cam;

    private bool isGrounded;
    private float timer;
    private bool canThrow = true;
    private float raycastDist = 0f;


    // Functions
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        // Cache the inputs
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Set player animation state
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        float speed;

        if ((hasHorizontalInput || hasVerticalInput) && (Input.GetKey("left shift") || Input.GetKey("right shift")))
        {
            speed = 1f; // Running
        }
        else if (hasHorizontalInput || hasVerticalInput)
        {
            speed = 0.5f; // Walking
        }
        else
        {
            speed = 0f; // Standing
        }

        bool isAiming = Input.GetMouseButton(1);
        bool isAttacking = Input.GetMouseButton(0);
        bool isCrouching = (Input.GetKey("left ctrl") || Input.GetKey("right ctrl"));

        m_Animator.SetFloat("Speed", speed);
        m_Animator.SetBool("Aim", isAiming);
        m_Animator.SetBool("Attack", isAttacking);
        m_Animator.SetBool("Crouch", isCrouching);

        // Jumping
        if (Input.GetKey("space") && isGrounded)
        {
            m_Rigidbody.velocity += jumpSpeed * Vector3.up;

            isGrounded = false;
        }

        // Weapons
        // TODO: Turn player when aiming and shooting
        if (Input.GetKey("g") && canThrow)
        {
            canThrow = false;
            timer = 0f;
            m_ThrowDirection = cam.transform.forward;
            m_ThrowDirection.Normalize();
            Vector3 throwStart = gameObject.transform.position + Vector3.up;

            GameObject grenade = (GameObject) Instantiate(grenadeObject, throwStart, Quaternion.identity);
            Physics.IgnoreCollision(m_Collider, grenade.GetComponent<Collider>());
//            grenade.GetComponent<Rigidbody>().velocity = transform.TransformDirection(m_ThrowDirection * throwVelocity);

            // TODO: Make throw target raycast ignore player collisions
            var camTransform = cam.transform;
            Ray ray = new Ray(camTransform.position, camTransform.forward);
            Physics.Raycast(ray, out var raycastHit);

            var m = new ThrowParameters(raycastHit.point, throwAngle, throwGravity);
            grenade.BroadcastMessage("Throw", m);

//            Debug.DrawRay(throwStart, m_ThrowDirection, Color.magenta, 3f);
        }

        MovementManagement(horizontal, vertical);
    }


    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 50), "Distance to object: " + raycastDist);
    }


    void OnDrawGizmos()
    {
        var camTransform = cam.transform;
        var camPosition = camTransform.position;
        Vector3 direction = camTransform.forward;
        Ray ray = new Ray(camPosition, direction);
        Debug.DrawRay(camPosition, direction);

        m_Collider.enabled = false; // Prevent the raycast from colliding with the player object
        if (Physics.Raycast(ray, out var raycastHit))
        {
            raycastDist = raycastHit.distance;
            Vector3 p = camTransform.TransformDirection(Vector3.forward) * raycastHit.distance;
            Debug.DrawRay(camPosition, p, Color.yellow);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(raycastHit.point, 0.1f);
        }

        m_Collider.enabled = true; // Reenable the player collider after the raycast
    }


    void Update()
    {
        timer += 1 * Time.deltaTime;

        if (timer >= grenadeTime)
        {
            canThrow = true;
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }


    /////////////////////////////////////////////CHARACTER MOVEMENT/////////////////////////////////////////

    void MovementManagement(float horizontal, float vertical)
    {
        // If there is some axis input...
        if (horizontal != 0f || vertical != 0f)
        {
            Rotating(horizontal, vertical);
        }
    }

    void Rotating(float horizontal, float vertical)
    {
        // Create a new vector of the horizontal and vertical inputs
        m_Movement = new Vector3(horizontal, 0f, vertical);
        m_Movement = Camera.main.transform.TransformDirection(m_Movement);
        m_Movement.y = 0.0f;
        m_Movement.Normalize();

        // Create a rotation based on this new vector assuming that up is the global y axis
        m_TargetRotation = Quaternion.LookRotation(m_Movement, Vector3.up);

        // Create a rotation that is an increment closer to the target rotation from the player's rotation
        m_Rotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, m_TargetRotation,
            turnSmoothing * Time.deltaTime);
    }

    void OnAnimatorMove()
    {
        // Move the player to the new position
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);

        // Rotate the player
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}