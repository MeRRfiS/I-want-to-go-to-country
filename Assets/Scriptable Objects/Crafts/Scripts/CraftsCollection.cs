using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafts Collection", menuName = "Craft System/Crafts")]
public class CraftsCollection : ScriptableObject
{
    public List<CraftModel> _craftedItemsList;
}

[System.Serializable]
public class CraftModel
{
    public string _name;
    public List<CraftedItem> _recipe;
    public Item _creftedItem;
}

[System.Serializable]
public class CraftedItem
{
    public Item _item;
    public int _amount;
}
