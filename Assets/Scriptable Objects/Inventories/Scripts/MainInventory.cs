using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory/Main")]
public class MainInventory : Inventory
{
    public override void Init()
    {
        Container = new Item[GlobalConstants.MAX_ITEMS_IN_INVENTORY];
    }

    private bool GetItemIndex(Item item, out int index)
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (Container[i] == null) continue;
            if (Container[i]._id == item._id && Container[i].Amount != 100)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool GetEmptyCell(out int index)
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (Container[i] == null)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool AddNewValuesToArray(Item item, int amount)
    {
        while (amount > 0)
        {
            int index;
            if (!GetEmptyCell(out index)) return false;

            Container[index] = item.Copy();
            Container[index].Amount = amount > GlobalConstants.MAX_ITEM_IN_CELL ?
                                      GlobalConstants.MAX_ITEM_IN_CELL : amount;
            amount -= 100;
            //ToDo: Make throwing items if inventory full
        }

        return true;
    }

    public override bool AddItem(InventorySlot newItem)
    {
        int index;
        switch (newItem.Item._type)
        {
            case ItemTypeEnum.None:
                return false;
            case ItemTypeEnum.Instrument:
            case ItemTypeEnum.Fertilizers:
                if (!GetEmptyCell(out index)) return false;

                Container[index] = newItem.Item;
                break;
            case ItemTypeEnum.Seed:
            case ItemTypeEnum.Tree:
            case ItemTypeEnum.Harvest:
                if (GetItemIndex(newItem.Item, out index))
                {
                    if (Container[index].Amount + newItem.Amount > GlobalConstants.MAX_ITEM_IN_CELL)
                    {
                        newItem.Amount -= GlobalConstants.MAX_ITEM_IN_CELL - Container[index].Amount;
                        Container[index].Amount = GlobalConstants.MAX_ITEM_IN_CELL;
                        if (!AddNewValuesToArray(newItem.Item, newItem.Amount)) return false;
                    }
                    else
                    {
                        Container[index].Amount += newItem.Amount;
                    }
                }
                else
                {
                    if (!AddNewValuesToArray(newItem.Item, newItem.Amount)) return false;
                }
                break;
        }

        return true;
    }
}
