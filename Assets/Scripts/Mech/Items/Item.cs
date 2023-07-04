using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int Id { get; set; }
    public int Price { get; set; }
    public abstract void Use();
}
