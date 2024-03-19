using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : ScriptableObject
{
    [field: SerializeField] public ItemController Object { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsCanSold { get; private set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public ItemTypeEnum Type { get; private set; }
    [HideInInspector] public bool IsDroped { get; set; } = false;
    public int Amount { get; set; }

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
        clone.Id = Id;
        clone.Price = Price;
        clone.Amount = Amount;
        clone.Type = Type;

        return clone;
    }
}
