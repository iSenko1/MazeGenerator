using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    public bool isNavMeshReady = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isNavMeshReady)
        {
            navMeshAgent.destination = movePositionTransform.position;
        }
    }


}
