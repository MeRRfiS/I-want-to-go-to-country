using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _itemList;
    [SerializeField] private Image _resultItemIcon;

    [Header("Prefab")]
    [SerializeField] private GameObject _itemInfo;

    private List<int> _amountOfItems = new List<int>();
    private CraftModel _craft;
    private BuildController _controller;

    public void CraftItem()
    {
        bool isCanCraft = _controller.IsCanDoSomething(_craft, _amountOfItems);
        if (isCanCraft)
        {
            for (int i = 0; i < _craft._neededItems.Count; i++)
            {
                InventoryController.GetInstance().RemoveItem(_craft._neededItems[i]._item, _amountOfItems[i]);
            }
            InventoryController.GetInstance().AddItemToMainInventory(_craft._creftedItem, 1);
            UIController.GetInstance().RedrawInventories();
        }
    }

    public void DrawCraftInformation(CraftModel craft, BuildController controller)
    {
        _resultItemIcon.sprite = craft._creftedItem.Icon;
        _amountOfItems.Clear();
        for (int i = 0; i < _itemList.childCount; i++)
        {
            Destroy(_itemList.GetChild(i).gameObject);
        }

        _craft = craft;
        _controller = controller; 
        for (int i = 0; i < _craft._neededItems.Count; i++)
        {
            int amount = _craft._neededItems[i]._amount;
            if(amount != 1) 
            {
                _amountOfItems.Add((int)Mathf.Floor(amount - amount * _controller.DecreaseCountMultipler));
            }
            else
            {
                _amountOfItems.Add(amount);
            }
        }
        for (int i = 0; i < _craft._neededItems.Count; i++)
        {
            ItemInformation craftInformation = Instantiate(_itemInfo, _itemList).GetComponent<ItemInformation>();
            craftInformation.DrawItemInformation(_craft._neededItems[i], _amountOfItems[i]);
        }
    }
}
