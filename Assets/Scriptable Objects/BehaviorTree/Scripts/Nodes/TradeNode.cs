using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeNode : ActionNode
{
    private StateEnum _state;

    protected override void OnStart()
    {
        NPC.Shop.enabled = true;
        NPC.Shop.OnBuyItem += OnBuyItem;
        NPC.Animation.OnStopHappy += MakeNormalFace;
        _state = StateEnum.Running;
    }

    protected override void OnStop()
    {
        NPC.Shop.enabled = false;
        NPC.Shop.OnBuyItem -= OnBuyItem;
        NPC.Animation.OnStopHappy -= MakeNormalFace;
    }

    protected override StateEnum OnUpdate()
    {
        if (NPC.IsHold) _state = StateEnum.Success;
        if (Vector3.Distance(NPC.transform.position, NPC.ShopPosition.position) > 2f) _state = StateEnum.Success;

        return _state;
    }

    private void OnBuyItem()
    {
        NPC.Animation.IsHappy(true);
        NPC.Face.HappyFace();
    }

    private void MakeNormalFace()
    {
        NPC.Face.NormalFace();
    }
}
