using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LocationNode : ActionNode
{
    private enum TypeWalk
    {
        Free = 0,
        Home,
        Shop
    }

    [SerializeField] private TypeWalk _typeWalk;

    private const float RADIUS = 50;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override StateEnum OnUpdate()
    {
        StateEnum state = StateEnum.Running;
        if (NPC.IsHold) return StateEnum.Success;

        switch (_typeWalk)
        {
            case TypeWalk.Free:
                if (NPC.Agent.remainingDistance <= NPC.Agent.stoppingDistance)
                {
                    NPC.Agent.SetDestination(GetNewRandomPoint());

                    state = StateEnum.Success;
                }
                else
                {
                    state = StateEnum.Running;
                }
                break;
            case TypeWalk.Home:
                NPC.Agent.SetDestination(NPC.HomePosition.position);
                if (!NPC.Agent.pathPending)
                {
                    if (NPC.Agent.remainingDistance <= NPC.Agent.stoppingDistance)
                    {
                        state = StateEnum.Success;
                    }
                    else
                    {
                        state = StateEnum.Running;
                    }
                }
                break;
            case TypeWalk.Shop:
                NPC.Agent.SetDestination(NPC.ShopPosition.position);
                if (!NPC.Agent.pathPending)
                {
                    if (NPC.Agent.remainingDistance <= NPC.Agent.stoppingDistance)
                    {
                        state = StateEnum.Success;
                    }
                    else
                    {
                        state = StateEnum.Running;
                    }
                }
                break;
        }

        return state;
    }

    private Vector3 GetNewRandomPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * RADIUS + Blackboard.Road.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, RADIUS, new NavMeshQueryFilter { agentTypeID = Blackboard.Surface.agentTypeID, areaMask = 1 });

        return hit.position;
    }
}
