using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterNode : ActionNode
{
    protected override void OnStart()
    {
        NPC.Animation.OnDead += BecomeDead;
    }

    protected override void OnStop()
    {
        NPC.Animation.OnDead -= BecomeDead;
    }

    protected override StateEnum OnUpdate()
    {
        if(NPC.transform.position.y >= Blackboard.WaterLevel)
        {
            return StateEnum.Success;
        }

        NPC.IsDead = true;
        NPC.Face.AngryFace();
        NPC.Animation.IsDead(true);

        return StateEnum.Running;
    }

    private void BecomeDead()
    {
        RuntimeManager.PlayOneShot(FMODEvents.instance.Explosion, NPC.NpcObject.transform.position);
        Instantiate(Blackboard.Explosion, NPC.transform.position, Quaternion.identity);
        NPC.Agent.Warp(NPC.HomePosition.position);
        NPC.IsDead = false;
    }
}
