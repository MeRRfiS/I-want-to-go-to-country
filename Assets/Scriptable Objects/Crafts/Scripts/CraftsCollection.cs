using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafts Collection", menuName = "Craft System/Crafts")]
public class CraftsCollection : ScriptableObject
{
    public List<CraftModel> _craftedItemsList;
}

[System.Serializable]
public class CraftModel: InformationModel
{
    public Item _creftedItem;
}
