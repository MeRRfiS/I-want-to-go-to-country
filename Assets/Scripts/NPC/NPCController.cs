using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshSurface _surface;
    [SerializeField] private GameObject _npcObject;
    [SerializeField] private NPCFace _face;
    [SerializeField] private Transform _road;

    private bool _isOnHand = false;

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
        NavMesh.SamplePosition(randomPos, out hit, 50, new NavMeshQueryFilter { agentTypeID = _surface.agentTypeID, areaMask = 1 });

        return hit.position;
    }

    public void GettingToHand()
    {
        //TODO: Change when was created behavior tree
        if(!_isOnHand)
        {
            _agent.enabled = false;
            _isOnHand = true;
            _npcObject.layer = LayerMask.NameToLayer(LayerConstants.ITEM);
            enabled = false;
            _face.AngryFace();
            HandsAnimationManager.GetInstance().IsHoldNPC(true);
        }
        else
        {
            _agent.enabled = true;
            _isOnHand = false;
            _npcObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
            enabled = true;
            _face.NormalFace();
            GetNewRandomPoint();
            HandsAnimationManager.GetInstance().IsHoldNPC(false);
        }
    }
}
