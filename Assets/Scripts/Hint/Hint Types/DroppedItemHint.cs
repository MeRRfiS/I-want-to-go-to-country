using UnityEngine;

public class DroppedItemHint : HintBase
{
    [SerializeField] private ItemController _item;

    public override string GetText()
    {
        return _data.GetText(_item.Item.name, _item.Item.Amount);
    }
}