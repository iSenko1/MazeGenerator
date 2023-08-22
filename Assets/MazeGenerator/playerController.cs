using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class playerController : MonoBehaviour
{
    private InputManager controls;
    private float moveSpeed = 6f;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private Vector2 move;
    private float jumpHeight =  2.4f;
    private CharacterController controller;
    public Transform ground;
    public float distanceToGround = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    public AudioClip JumpSound = null;
    public AudioClip HitSound = null;
    public AudioClip CoinSound = null;
    private AudioSource mAudioSource = null;
    private Rigidbody mRigidBody = null;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        mRigidBody = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        controls = new InputManager();
        controller = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        //Grav();
        PlayerMovement();
        Jump();
    }

    /*    private void Grav()
        {
            isGrounded = Physics.CheckSphere(ground.position,distanceToGround,goundMask);
            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    */
    private void PlayerMovement()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();
        Vector3 movement = (move.y * transform.forward) + (move.x * transform.right);
        controller.Move(movement * moveSpeed * Time.deltaTime);

        // Check if player is moving
        if (move.sqrMagnitude > 0.01) // using sqrMagnitude for efficiency, comparing with a small number (0.01) to account for minor input values
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }


    private void Jump()
    {
        if (mRigidBody != null)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (mAudioSource != null && JumpSound != null)
                {
                    mAudioSource.PlayOneShot(JumpSound);
                }
                mRigidBody.AddForce(Vector3.up * 200);
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    
    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Coin"))
        {
            
            Destroy(other.gameObject);
        }
    }*/

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Coin"))
        {
            if (mAudioSource != null && CoinSound != null)
            {
                mAudioSource.PlayOneShot(CoinSound);
            }
            Destroy(other.gameObject);
        }
    }
}
