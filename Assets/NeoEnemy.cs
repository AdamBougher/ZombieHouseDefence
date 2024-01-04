using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeoEnemy : MonoBehaviour
{
    Vector3 target;
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = FindObjectOfType<Player>().transform.position;
    }

    private void Update()
    {
        SetAgentPosition();
    }

    private void SetAgentPosition()
    {
        target = FindObjectOfType<Player>().transform.position;
        agent.SetDestination(new Vector3(target.x, target.y, 0));
    }

}
