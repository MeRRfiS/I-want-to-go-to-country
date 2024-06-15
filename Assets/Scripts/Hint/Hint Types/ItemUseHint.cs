using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemUseHint : HintBase
{
    [SerializeField] private Item[] _requireditems;

    public override bool IsActive()
    {
        return IsItemInHand(_requireditems);
    }

    public static bool IsItemInHand(IEnumerable<Item> requiredItems)
    {
        var inventory = InventoryController.GetInstance();
        int activeId = inventory.PlayerItems[inventory.ActivePlayerItemIndex]?.Id ?? -1;
        return requiredItems.Any(x => x.Id == activeId);
    }
}