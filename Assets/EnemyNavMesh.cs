using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    [SerializeField] private float attackDistance = 3.0f;  // distance within which to attack the player
    [SerializeField] private float attackRate = 1.0f;  // attacks per second

    private NavMeshAgent navMeshAgent;
    private Animator animator;  // assume you have an Animator attached to the enemy
    BoxCollider boxCollider;
    private Transform player;  // to keep a reference to the player
    private float nextAttackTime;
    private bool hasAttackedThisCycle = false; // New field to track whether the enemy has attacked during the current animation cycle
    private CharacterController playerCharacterController; // Reference to the player's CharacterController component


    public bool isNavMeshReady = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();  // initialize the Animator

        // Find the player's Transform
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj)
        {
            player = playerObj.transform;
        }
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Add this method to handle scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the main menu scene is loaded, destroy this GameObject
        if (scene.buildIndex == 0)  // Assuming the main menu is at build index 0
        {
            Destroy(gameObject);
        }
    }
    // Unsubscribe when this GameObject gets destroyed
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        nextAttackTime = 0.5f;  // initialize next attack time
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isNavMeshReady)
        {
            if (player == null)
            {
                // What the enemy should do if the player is no longer available
                // For now, let's just stop the enemy
                navMeshAgent.isStopped = true;
                return;
            }

            if (player)
            {
                // Calculate distance to player
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // If within attack range, attack player and stop movement
                if (distanceToPlayer <= attackDistance)
                {
                    Attack();
                    navMeshAgent.isStopped = true;
                    animator.SetBool("isMoving", false); // Make sure to set isMoving to false when attacking or idle
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.destination = movePositionTransform.position;

                    // Set isMoving for the Animator
                    if (navMeshAgent.velocity.sqrMagnitude > 0.01)
                    {
                        //Debug.Log("Enemy is moving.");
                        animator.SetBool("isMoving", true);
                    }
                    else
                    {
                        animator.SetBool("isMoving", false);
                    }
                }
            }
            else
            {
                navMeshAgent.destination = movePositionTransform.position;
                if (navMeshAgent.velocity.sqrMagnitude > 0.01)
                {
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    animator.SetBool("isMoving", false);
                }
            }
        }
    }


    private void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ZombieAttack"))
        {
            animator.SetTrigger("Attack");
            hasAttackedThisCycle = false; // Reset the attack flag whenever a new attack is initiated
        }
        /*
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");

            // Update the next attack time
            nextAttackTime = Time.time + 1.0f / attackRate;
        }
        */
    }

    void EnableAttack()
    {
        boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj)
        {
            playerCharacterController = playerObj.GetComponent<CharacterController>();
        }
        // If we have already attacked this cycle, return immediately
        if (hasAttackedThisCycle)
        {
            return;
        }

        var player = other.GetComponent<playerController>();
        var healthComponent = other.GetComponent<Health>();

        if (healthComponent != null)
        {
            Debug.Log("HIT!!!");
            healthComponent.TakeDamage(1);
            hasAttackedThisCycle = true;

            // Call Respawn function if health is above 0
            if (healthComponent.currentHealth > 0)
            {
                playerCharacterController.enabled = false;
                player.Respawn();
                playerCharacterController.enabled = true;
            }
            else // Load main menu if no more health
            {
                Debug.Log("Game Over, loading main menu...");
                GameManager.instance.gameIsRunning = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
            }
        }
    }
}
