using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [field: SerializeField] public ItemController Object { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    public bool _isCanSold;
    [HideInInspector] public bool _isDroped = false;
    public int _id;
    public int _price;
    protected int _amount;
    public ItemTypeEnum _type;

    public int Amount
    {
        get => _amount;
        set => _amount = value;
    }

    public virtual void UseItem() { }

    public virtual bool IsItemCountZero()
    {
        if (Amount == 0) return true;

        return false;
    }

    public virtual void Init() { }
    public virtual void Destruct() { }
    public virtual GameObject Updating(GameObject obj, GameObject prefab) => null;
    public virtual GameObject StopUpdating() => null;

    public Item Copy()
    {
        Item clone = (Item) this.MemberwiseClone();
        clone._id = _id;
        clone._price = _price;
        clone._amount = Amount;
        clone._type = _type;

        return clone;
    }
}
