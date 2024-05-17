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
            NPC.NpcObject.layer = LayerMask.NameToLayer(LayerConstants.ITEM);
            NPC.Face.AngryFace();

            NPC.transform.localPosition = new Vector3(0, -0.003f, -0.001f);
            NPC.transform.localRotation = Quaternion.identity;

            HandsAnimationManager.GetInstance().IsHoldNPC(true);
        }
        else
        {
            _state = StateEnum.Success;
            NPC.Agent.enabled = true;
            NPC.NpcObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
            NPC.Face.NormalFace();
        }

        return _state;
    }
}
