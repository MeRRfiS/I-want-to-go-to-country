using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument: Item
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int MaxDurability { get; private set; }
    [field: SerializeField] public InstrumentTypeEnum InstrumentType { get; private set; }

    protected int _durability = 0;

    public int Durability
    {
        get => _durability;
    }

    public override bool IsItemCountZero()
    {
        if (Durability == 0) return true;

        return false;
    }

    public override void GetItemInHand()
    {
        IsInHand = true;
        HandsAnimationManager.GetInstance().IsHoldInst(true);
        HandsAnimationManager.GetInstance().IsHoldFunnel(false);
        HandsAnimationManager.GetInstance().IsHoldStaf(false);
    }
}
