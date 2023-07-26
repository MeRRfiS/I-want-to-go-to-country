using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument: Item
{
    public int Level { get; set; }
    public int Durability { get; set; }

    public static Item CreateInstrument(InstrumentTypeEnum type, int level, int durability)
    {
        switch (type)
        {
            case InstrumentTypeEnum.Hoe:
                return new Hoe(level, durability);
            case InstrumentTypeEnum.Axe:
                return new Axe(level, durability);
            case InstrumentTypeEnum.Funnel:
                return new Funnel(level, durability);
            case InstrumentTypeEnum.None:
            default:
                return null;
        }
    }
}
