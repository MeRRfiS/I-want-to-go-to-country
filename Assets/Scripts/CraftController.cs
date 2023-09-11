using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CraftsCollection _collection;

    private bool IsHasCraftedtItem(List<Item> items, CraftModel craft)
    {
        foreach (var craftedItem in craft._recipe)
        {
            List<Item> neededItems = items.Where(item =>
            {
                if (item == null) return false;
                if (item._id != craftedItem._item._id) return false;

                return true;
            }).ToList();
            if (neededItems.Count == 0) return false;

            int itemsAmount = neededItems.Select(item => item.Amount).Sum();
            if (itemsAmount < craftedItem._amount) return false;
        }

        return true;
    }

    public void LoadCraftsCollectionToUI()
    {
        UIController.GetInstance().RedrawCraftMenu(_collection._craftedItemsList, this);
        UIController.GetInstance().SwitchActiveCraftMenu();
    }

    public bool IsCanCraft(CraftModel craft)
    {
        Item[] inventoryItems = InventoryController.GetInstance().ItemsArray;
        Item[] playerItems = InventoryController.GetInstance().PlayerItems;
        List<Item> allItems = new List<Item>();
        foreach (var item in inventoryItems)
        {
            allItems.Add(item);
        }
        foreach (var item in playerItems)
        {
            allItems.Add(item);
        }

        bool isHasItem = IsHasCraftedtItem(allItems, craft);

        return isHasItem;
    }
}
