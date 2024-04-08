using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _road;

    private void Update()
    {
        SetNewDestination();
    }

    private void SetNewDestination()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Vector3 point = GetNewRandomPoint();
            _agent.SetDestination(point);
        }
    }

    private Vector3 GetNewRandomPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * 50 + _road.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, 50, NavMesh.AllAreas);

        return hit.position;
    }
}
