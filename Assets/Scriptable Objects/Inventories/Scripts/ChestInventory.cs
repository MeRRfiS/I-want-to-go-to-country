using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : Inventory
{
    public override void Init()
    {
        Container = new Item[MechConstants.MAX_ITEMS_IN_CHEST];
    }
}
