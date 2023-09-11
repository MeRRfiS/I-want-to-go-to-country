using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class CraftHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _itemList;

    [Header("Prefab")]
    [SerializeField] private GameObject _itemInfo;

    private CraftModel _craft;
    private CraftController _controller;

    public void CraftItem()
    {
        bool isCanCraft = _controller.IsCanCraft(_craft);
        if (isCanCraft)
        {
            foreach (var craftedItem in _craft._recipe)
            {
                InventoryController.GetInstance().RemoveItem(craftedItem._item, craftedItem._amount);
            }
            InventoryController.GetInstance().AddItemToMainInventory(_craft._creftedItem, 1);
            UIController.GetInstance().RedrawInventories();
        }
    }

    public void DrawCraftInformation(CraftModel craft, CraftController controller)
    {
        for (int i = 0; i < _itemList.childCount; i++)
        {
            Destroy(_itemList.GetChild(i).gameObject);
        }

        _craft = craft;
        _controller = controller;
        foreach (var craftItem in craft._recipe)
        {
            CraftItemInformation craftInformation = Instantiate(_itemInfo, _itemList).GetComponent<CraftItemInformation>();
            craftInformation.DrawCraftItemInformation(craftItem);
        }
    }
}
