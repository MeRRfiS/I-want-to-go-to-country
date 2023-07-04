using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Instrument: Item
{
    public int Level { get; set; }
    public int Durability { get; set; }

    public override void Use()
    {
        UseInstrument();
    }

    protected abstract void UseInstrument();
}
