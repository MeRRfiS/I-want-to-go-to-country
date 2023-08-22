using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory/Player")]
public class PlayerInventory : Inventory
{
    public override void Init()
    {
        Container = new Item[GlobalConstants.MAX_ITEMS_IN_PLAYER];
    }

    public override bool AddItem(InventorySlot newItem)
    {
        throw new System.NotImplementedException();
    }
}
