using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float moveSpeed = 5f; // Speed at which the enemy moves towards the player
    public float spawnDistance = 10f; // Distance from the player where the enemy should spawn
    private Animator animator;
    private float groundCheckDistance = 1.5f; // Set an appropriate value based on your enemy size
    public LayerMask groundLayer; // Set this to whatever layer your ground is on. Usually, it's the "Default" layer.
    public float enemyHeight = 2f; // Adjust based on your enemy's height



    private void Start()
    {
        animator = GetComponent<Animator>();
        // If player isn't set manually in the Inspector, try to find it by tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Position the enemy near the player when spawned
        PositionEnemyNearPlayer();
    }

    private void PositionEnemyNearPlayer()
    {
        if (player != null)
        {
            Vector3 randomDirection = Random.onUnitSphere; // get a random direction
            randomDirection.y = 0; // keep the enemy on the same horizontal plane as the player
            Vector3 spawnPosition = player.position + randomDirection.normalized * spawnDistance;
            transform.position = spawnPosition;
        }
    }

    private void Update()
    {
        GroundCheck();
        if (player != null)
        {
            // Make the enemy look at the player
            Vector3 direction = (player.position - transform.position).normalized;

            // If the enemy is moving, set isMoving to true, otherwise false
            animator.SetBool("isMoving", direction.magnitude > 0.01f);

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Move the enemy towards the player
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, groundCheckDistance, groundLayer))
        {
            transform.position = hit.point; // Place the enemy on the ground
                                            // Adjust if necessary depending on where the enemy's pivot is.
            transform.position += new Vector3(0, enemyHeight / 2, 0);
        }
    }
}
