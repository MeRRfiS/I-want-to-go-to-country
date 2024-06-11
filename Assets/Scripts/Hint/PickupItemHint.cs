using UnityEngine;

public class PickupItemHint : Hint
{
    [SerializeField] private ItemController _item;

    public override string GetText()
    {
        return _data.GetText(_item.Item.name);
    }
}