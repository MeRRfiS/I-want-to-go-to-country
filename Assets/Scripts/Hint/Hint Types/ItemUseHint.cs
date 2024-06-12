using System.Linq;
using UnityEngine;

public class ItemUseHint : HintBase
{
    [SerializeField] private Item[] _requireditems;

    public override string GetText()
    {
        var inventory = InventoryController.GetInstance();
        int activeId = inventory.PlayerItems[inventory.ActivePlayerItemIndex]?.Id ?? -1;
        if (_requireditems.Any(x => x.Id == activeId))
        {
            return base.GetText();
        }
        else
        {
            return "No requider item";
        }
    }
}