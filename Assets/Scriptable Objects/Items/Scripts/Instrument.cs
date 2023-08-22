using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument: Item
{
    public int _level;
    public int _maxDurability;
    protected int _durability;
    public InstrumentTypeEnum _instrumentType;

    public int Durability
    {
        get => _durability;
    }

    public override bool IsItemCountZero()
    {
        if (Durability == 0) return true;

        return false;
    }

    //public static Item CreateInstrument(InstrumentTypeEnum type, int level, int durability)
    //{
    //    switch (type)
    //    {
    //        case InstrumentTypeEnum.Hoe:
    //            return new Hoe();
    //        case InstrumentTypeEnum.Axe:
    //            return new Axe(level, durability);
    //        case InstrumentTypeEnum.Funnel:
    //            return new Funnel(level, durability);
    //        case InstrumentTypeEnum.None:
    //        default:
    //            return null;
    //    }
    //}
}
