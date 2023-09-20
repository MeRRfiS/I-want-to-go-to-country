using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory/Player")]
public class PlayerInventory : Inventory
{
    public override void Init()
    {
        Container = new Item[MechConstants.MAX_ITEMS_IN_PLAYER];
    }
}
