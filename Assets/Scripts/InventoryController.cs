using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class InventoryController : MonoBehaviour
{
    private static InventoryController instance;

    private Item[] _itemsArray = new Item[GlobalConstants.MAX_ITEMS_IN_INVENTORY];

    public static InventoryController GetInstance() => instance;

    public Item[] ItemsArray
    {
        get => _itemsArray;
    }
 
    private int GetItemIndex(Item item)
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (_itemsArray[i] == null) continue;
            if (_itemsArray[i].Id == item.Id && _itemsArray[i].Count != 100) return i;
        }

        return -1;
    }

    private int GetEmptyCell()
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (_itemsArray[i] == null) return i;
        }

        return -1;
    }

    private bool AddNewValuesToArray(Item item, int value)
    {
        while (value > 0)
        {
            int index = GetEmptyCell();
            if (index == -1) return false;

            _itemsArray[index] = item.Copy();
            _itemsArray[index].Count = value > GlobalConstants.MAX_ITEM_IN_CELL ?
                                          GlobalConstants.MAX_ITEM_IN_CELL : value;
            value -= 100;
            //ToDo: Make throwing items if inventory full
        }

        return true;
    }

    private void Awake()
    {
        instance = this;
    }

    public bool AddItem(Item item, int value)
    {
        int index = 0;
        switch (item.Type)
        {
            case ItemTypeEnum.None:
                return false;
            case ItemTypeEnum.Instrument:
                index = GetEmptyCell();
                if (index == -1) return false;

                _itemsArray[index] = item.Copy();
                break;
            case ItemTypeEnum.Seed:
            case ItemTypeEnum.Tree:
            case ItemTypeEnum.Harvest:
                index = GetItemIndex(item);
                if (index != -1)
                {
                    if (_itemsArray[index].Count + value > GlobalConstants.MAX_ITEM_IN_CELL)
                    {
                        value -= GlobalConstants.MAX_ITEM_IN_CELL - _itemsArray[index].Count;
                        _itemsArray[index].Count = GlobalConstants.MAX_ITEM_IN_CELL;
                        if(!AddNewValuesToArray(item, value)) return false;
                    }
                    else
                    {
                        _itemsArray[index].Count += value;
                    }
                }
                else
                {
                    if (!AddNewValuesToArray(item, value)) return false;
                }
                break;
        }

        return true;
    }

    public void RemoveItem(int id) 
    { 
    
    }
}
