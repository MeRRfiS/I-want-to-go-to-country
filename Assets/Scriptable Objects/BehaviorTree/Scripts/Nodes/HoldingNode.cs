using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingNode : ActionNode
{
    private StateEnum _state;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override StateEnum OnUpdate()
    {
        _state = StateEnum.Success;

        if (NPC.IsHold)
        {
            _state = StateEnum.Running;
            NPC.Agent.enabled = false;
            NPC.gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
            NPC.NpcObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
            NPC.Face.AngryFace();
            NPC.Animation.IsAngry(true);

            NPC.transform.localPosition = new Vector3(0, -0.006f, 0);
            NPC.transform.localRotation = Quaternion.identity;

            HandsAnimationManager.GetInstance().IsHoldNPC(true);
        }
        else
        {
            _state = StateEnum.Success;
            NPC.Agent.enabled = true;
            NPC.gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
            NPC.NpcObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
            NPC.Face.NormalFace();
            NPC.Animation.IsAngry(false);
        }

        return _state;
    }
}
