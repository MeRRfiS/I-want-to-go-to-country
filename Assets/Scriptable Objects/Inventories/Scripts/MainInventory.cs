using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory/Main")]
public class MainInventory : Inventory
{
    public override void Init()
    {
        Container = new Item[MechConstants.MAX_ITEMS_IN_INVENTORY];
    }

    private bool GetItemIndex(Item item, out int index)
    {
        for (int i = 0; i < MechConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (Container[i] == null) continue;
            if (Container[i].Id == item.Id && Container[i].Amount != MechConstants.MAX_ITEM_IN_CELL)
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
        for (int i = 0; i < MechConstants.MAX_ITEMS_IN_INVENTORY; i++)
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

    private bool AddNewValuesToArray(Item item, int amount, Vector3? possibleDropPosition)
    {
        while (amount > 0)
        {
            int index;
            if (!GetEmptyCell(out index))
            {
                if(!item.IsDroped) InventoryController.GetInstance().DropItemFromInventory(item, amount, possibleDropPosition);
                return false;
            }

            item.IsDroped = false;
            Container[index] = item;
            Container[index].Amount = amount > MechConstants.MAX_ITEM_IN_CELL ?
                                      MechConstants.MAX_ITEM_IN_CELL : amount;
            amount -= 100;
            //ToDo: Make throwing items if inventory full
        }

        return true;
    }

    public override bool AddItem(InventorySlot newItem, Vector3? possibleDropPosition = null)
    {
        int index;
        switch (newItem.Item.Type)
        {
            case ItemTypeEnum.None:
                return false;
            case ItemTypeEnum.Instrument:
            case ItemTypeEnum.Fertilizers:
            case ItemTypeEnum.Building:
                if (!GetEmptyCell(out index)) 
                {
                    if (!newItem.Item.IsDroped) InventoryController.GetInstance().DropItemFromInventory(newItem.Item, dropPosition: possibleDropPosition);
                    return false;
                }

                newItem.Item.IsDroped = false;
                Container[index] = newItem.Item;
                break;
            case ItemTypeEnum.Seed:
            case ItemTypeEnum.Tree:
            case ItemTypeEnum.Harvest:
                if (GetItemIndex(newItem.Item, out index))
                {
                    if (Container[index].Amount + newItem.Amount > MechConstants.MAX_ITEM_IN_CELL)
                    {
                        newItem.Amount -= MechConstants.MAX_ITEM_IN_CELL - Container[index].Amount;
                        Container[index].Amount = MechConstants.MAX_ITEM_IN_CELL;
                        if (!AddNewValuesToArray(newItem.Item, newItem.Amount, possibleDropPosition)) return false;
                    }
                    else
                    {
                        Container[index].Amount += newItem.Amount;
                    }
                }
                else
                {
                    if (!AddNewValuesToArray(newItem.Item, newItem.Amount, possibleDropPosition)) return false;
                }
                break;
        }

        return true;
    }
}
