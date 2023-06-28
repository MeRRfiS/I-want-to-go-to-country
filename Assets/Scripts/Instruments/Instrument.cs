using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Instrument
{
    public abstract GameObject CreateObj(GameObject obj = null, GameObject prefab = null);
    public abstract GameObject DestroyObj(GameObject obj = null);
    public abstract void Use();
}
