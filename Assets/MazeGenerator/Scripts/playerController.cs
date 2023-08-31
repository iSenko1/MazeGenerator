using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    private InputManager controls;
    private float moveSpeed = 6f;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private Vector2 move;
    private float jumpHeight = 1.5f;
    private CharacterController controller;
    public Transform ground;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private float playerYVelocity;
    public Vector3 startPosition;
    public AudioClip JumpSound = null;
    public AudioClip HitSound = null;
    public AudioClip CoinSound = null;
    private AudioSource mAudioSource = null;
    private Animator animator;
    [SerializeField]
    //private float startTime;
    private float playerStartTime;
    public ShortestPathAlgorithm shortestPathAlgorithm;

    void Start()
    {
        float currentHighscore = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
        Debug.Log("Current Highscore: " + currentHighscore);
        //ResetHighScore();
        playerYVelocity = 0f;
        animator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
        startPosition = transform.position;
        playerStartTime = Time.time;
        controls.Player.Jump.performed += ctx => {
            //Debug.Log("Jump input detected");
            TryJump();
        };
    }

    void Awake()
    {
        controls = new InputManager();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMovement();
        GravityLogic();
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
        */
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ground.position, groundCheckRadius);
    }

    private void PlayerMovement()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();
        Vector3 movement = (move.y * transform.forward) + (move.x * transform.right);
        controller.Move(movement * moveSpeed * Time.deltaTime);

        if (move.sqrMagnitude > 0.01)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void GravityLogic()
    {
        Collider[] groundHits = Physics.OverlapSphere(ground.position, groundCheckRadius, groundMask);

        isGrounded = Physics.Raycast(transform.position, -Vector3.up, controller.height / 2 + 0.1f, groundMask);

        foreach (Collider hit in groundHits)
        {
            //Debug.Log("Colliding with: " + hit.gameObject.name);

            // Make sure we didn't hit the player itself
            if (hit.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        //Debug.Log("Is Grounded: " + isGrounded);

        if (isGrounded && playerYVelocity < 0)
        {
            playerYVelocity = -2f; // Reset it to a small negative value to ensure the player sticks to the ground a bit

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                playerYVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //Debug.Log("Y Velocity on Jump: " + playerYVelocity);
            }
        }
        else
        {
            playerYVelocity += gravity * Time.deltaTime;
        }

        controller.Move(new Vector3(0, playerYVelocity, 0) * Time.deltaTime);
    }


    void TryJump()
    {
        if (isGrounded)
        {
            playerYVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (mAudioSource != null && JumpSound != null)
            {
                mAudioSource.PlayOneShot(JumpSound);
            }
        }
    }




    public void Respawn()
    {
        Debug.Log("Respawning player.");
        transform.position = startPosition;
        playerYVelocity = 0;
        StartCoroutine(DisableMovementTemporarily());
    }

    IEnumerator DisableMovementTemporarily()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(1f);
        moveSpeed = 6f;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Jump.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Jump.Disable();
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("Highscore");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Coin"))
        {
            //Respawn();
            if (mAudioSource != null && CoinSound != null)
            {
                mAudioSource.PlayOneShot(CoinSound);
            }
            Destroy(other.gameObject);
            GameManager.instance.AddPoint();

            if (shortestPathAlgorithm != null)
            {
                shortestPathAlgorithm.ShowBreadcrumbs();
            }
            else
            {
                Debug.LogError("ShortestPathAlgorithm is not set!");
            }
            
            controller.enabled = false;
            Respawn();
            controller.enabled = true;
            
        }

        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Triggered with enemy");
        }

        if (other.CompareTag("exitPrefab"))
        {
            //ResetHighScore();
            Debug.Log("YOU WON!");
            GameManager.instance.gameIsRunning = false;
            float timePlayed = Time.time - playerStartTime;
            Debug.Log("Time Played: " + timePlayed);

            // Update high score if current time is better
            float currentHighscore = PlayerPrefs.GetFloat("Highscore", float.MaxValue);
            Debug.Log("Current Highscore: " + currentHighscore);

            if (timePlayed < currentHighscore)
            {
                PlayerPrefs.SetFloat("Highscore", timePlayed);
                GameManager.instance.highscoreText.text = "HIGHSCORE: " + GameManager.instance.FormatTime(timePlayed); // Update displayed highscore.
                Debug.Log("New highscore set: " + timePlayed);
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
    }
}
