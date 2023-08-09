using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int Id { get; set; }
    public int Price { get; set; }
    public int Count { get; set; }
    public ItemTypeEnum Type { get; set; }

    public virtual void UseItem() { }

    public virtual bool IsItemCountZero() 
    { 
        if(Count == 0) return true;

        return false;
    }

    public virtual GameObject Updating(GameObject obj, GameObject prefab) => null;
    public virtual GameObject StopUpdating() => null;

    public Item Copy()
    {
        Item clone = (Item) this.MemberwiseClone();
        clone.Id = Id;
        clone.Price = Price;
        clone.Count = Count;
        clone.Type = Type;

        return clone;
    }
}
