using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsStorage : MonoBehaviour
{
    private static ItemsStorage _instance;
    
    public static ItemsStorage GetInstance() { return _instance; }

    [SerializeField] private List<ItemController> _items = new List<ItemController>();

    private void Awake()
    {
        _instance = this;
    }

    public ItemController GetItem(int id)
    {
        foreach (ItemController item in _items)
        {
            if(item.ItemInfo.Id == id) return item;
        }

        return null;
    }
}
