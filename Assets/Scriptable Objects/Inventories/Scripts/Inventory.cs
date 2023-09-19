using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject
{
    public Item[] Container { get; set; }

    public virtual void Init() { }
    public virtual bool AddItem(InventorySlot newItem) => false;


    public void MoveItem(int firstIndex, int secondIndex, Inventory toInventory)
    {
        Item temp = Container[firstIndex];
        Container[firstIndex] = toInventory.Container[secondIndex];
        toInventory.Container[secondIndex] = temp;
    }

    public void RemoveItem(Item removeItem, ref int amount)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i] == null) continue;
            if (Container[i]._id == removeItem._id)
            {
                int itemsAmount = Container[i].Amount;
                Container[i].Amount -= amount;
                if (Container[i].Amount <= 0)
                {
                    Container[i] = null;
                }
                amount -= itemsAmount;
                if (amount <= 0) 
                {
                    amount = 0;
                    return;
                }
            }
        }
    }

    public bool StackItem(int firstIndex, int secondIndex, Inventory toInventory)
    {
        if (toInventory.Container[secondIndex] == null)
            return false;

        if (toInventory.Container[secondIndex]._id != Container[firstIndex]._id)
            return false;

        if (toInventory.Container[secondIndex].Amount + Container[firstIndex].Amount > 
            GlobalConstants.MAX_ITEM_IN_CELL)
        {
            Container[firstIndex].Amount = (toInventory.Container[secondIndex].Amount +
                                            Container[firstIndex].Amount) -
                                            GlobalConstants.MAX_ITEM_IN_CELL;
            toInventory.Container[secondIndex].Amount = GlobalConstants.MAX_ITEM_IN_CELL;
            UIController.GetInstance().UpdatePinItemInfo();
        }
        else
        {
            toInventory.Container[secondIndex].Amount += Container[firstIndex].Amount;
            Container[firstIndex] = null;
            UIController.GetInstance().UnpinItemFromMouse();
        }

        return true;
    }
}

[System.Serializable]
public class InventorySlot
{
    public Item Item { get; set; }
    public int Amount { get; set; }

    public InventorySlot(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}
