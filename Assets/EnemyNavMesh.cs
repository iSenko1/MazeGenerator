using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    [SerializeField] private float attackDistance = 3.0f;  // distance within which to attack the player
    [SerializeField] private float attackRate = 1.0f;  // attacks per second

    private NavMeshAgent navMeshAgent;
    private Animator animator;  // assume you have an Animator attached to the enemy
    private Transform player;  // to keep a reference to the player
    private float nextAttackTime;

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
    }
    // Start is called before the first frame update
    void Start()
    {
        nextAttackTime = 0.5f;  // initialize next attack time
    }

    // Update is called once per frame
    void Update()
    {
        if (isNavMeshReady)
        {
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
                        Debug.Log("Enemy is moving.");  // Debugging line
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
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");

            // Update the next attack time
            nextAttackTime = Time.time + 1.0f / attackRate;
        }
    }


}
