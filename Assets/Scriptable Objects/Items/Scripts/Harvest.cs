using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Harvest Object", menuName = "Inventory System/Items/Harvest")]
public class Harvest : Item
{
    public override void Init()
    {
        Amount = 5;
    }
}
