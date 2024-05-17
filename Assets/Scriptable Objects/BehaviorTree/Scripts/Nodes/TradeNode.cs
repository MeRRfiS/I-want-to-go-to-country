using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeNode : ActionNode
{
    private StateEnum _state;

    protected override void OnStart()
    {
        NPC.Shop.enabled = true;
        _state = StateEnum.Running;
    }

    protected override void OnStop()
    {
        NPC.Shop.enabled = false;
    }

    protected override StateEnum OnUpdate()
    {
        if (NPC.IsHold) _state = StateEnum.Success;

        return _state;
    }
}
