using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject
{
    public Item[] Container { get; set; }

    public virtual void Init() { }
    public virtual bool AddItem(InventorySlot newItem) => false;

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
