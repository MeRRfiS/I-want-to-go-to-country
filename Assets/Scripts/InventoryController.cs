using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private struct SelectedItemInfo
    {
        public int _itemIndex;
        public CellTypeEnum _cellType;
    }

    private static InventoryController instance;

    private bool _isCanChangeActiveItem = true;
    private int _activePlayerItemIndex = 0;
    private SelectedItemInfo _selectedItemInfo;

    [Header("Inventory")]
    [SerializeField] private Inventory _mainInventory;
    [SerializeField] private Inventory _playerInventory;

    [Header("Components")]
    [SerializeField] private Transform _hand;

    public static InventoryController GetInstance() => instance;

    public bool IsCanChangeActiveItem
    {
        set => _isCanChangeActiveItem = value;
    }

    public Item[] ItemsArray
    {
        get => _mainInventory.Container;
    }

    public Item[] PlayerItems
    {
        get => _playerInventory.Container;
    }

    private bool ToStackItems(ref Item selectedItem, int indexOfSecondCell, CellTypeEnum type)
    {
        if (_selectedItemInfo._cellType == type &&
            _selectedItemInfo._itemIndex == indexOfSecondCell) return false;
        if (selectedItem == null) return false;
        if (selectedItem is Instrument || selectedItem is Fertilizers) return false;

        switch (type)
        {
            case CellTypeEnum.Inventory:
                if(ItemsArray[indexOfSecondCell] == null)
                    return false;

                if (ItemsArray[indexOfSecondCell]._id != selectedItem._id)
                    return false;

                if (ItemsArray[indexOfSecondCell].Amount + selectedItem.Amount > GlobalConstants.MAX_ITEM_IN_CELL)
                {
                    selectedItem.Amount = (ItemsArray[indexOfSecondCell].Amount + selectedItem.Amount) - 
                                          GlobalConstants.MAX_ITEM_IN_CELL;
                    ItemsArray[indexOfSecondCell].Amount = GlobalConstants.MAX_ITEM_IN_CELL;
                    UIController.GetInstance().UpdatePinItemInfo();
                }
                else
                {
                    ItemsArray[indexOfSecondCell].Amount += selectedItem.Amount;
                    selectedItem = null;
                    UIController.GetInstance().UnpinItemFromMouse();
                }
                break;
            case CellTypeEnum.Player:
                if (PlayerItems[indexOfSecondCell] == null)
                    return false;

                if (PlayerItems[indexOfSecondCell]._id != selectedItem._id)
                    return false;

                if (PlayerItems[indexOfSecondCell].Amount + selectedItem.Amount > GlobalConstants.MAX_ITEM_IN_CELL)
                {
                    selectedItem.Amount = (PlayerItems[indexOfSecondCell].Amount + selectedItem.Amount) - 
                                          GlobalConstants.MAX_ITEM_IN_CELL;
                    PlayerItems[indexOfSecondCell].Amount = GlobalConstants.MAX_ITEM_IN_CELL;
                    UIController.GetInstance().UpdatePinItemInfo();
                }
                else
                {
                    PlayerItems[indexOfSecondCell].Amount += selectedItem.Amount;
                    selectedItem = null;
                    _selectedItemInfo = new SelectedItemInfo();
                    UIController.GetInstance().UnpinItemFromMouse();
                }
                break;
        }

        return true;
    }

    private void MoveItemFromInventory(int indexOfSecondCell, CellTypeEnum type)
    {
        if (ItemsArray[_selectedItemInfo._itemIndex] == null) return;

        if (ToStackItems(ref ItemsArray[_selectedItemInfo._itemIndex], indexOfSecondCell, type)) return;
        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = ItemsArray[indexOfSecondCell];
                ItemsArray[indexOfSecondCell] = ItemsArray[_selectedItemInfo._itemIndex];
                ItemsArray[_selectedItemInfo._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = PlayerItems[indexOfSecondCell];
                PlayerItems[indexOfSecondCell] = ItemsArray[_selectedItemInfo._itemIndex];
                ItemsArray[_selectedItemInfo._itemIndex] = temp;
                break;
        }
        UIController.GetInstance().UnpinItemFromMouse();
        UIController.GetInstance().RedrawInventories();
        _selectedItemInfo = new SelectedItemInfo();
    }

    private void MoveItemFromPlayer(int indexOfSecondCell, CellTypeEnum type)
    {
        if (PlayerItems[_selectedItemInfo._itemIndex] == null) return;

        if (ToStackItems(ref PlayerItems[_selectedItemInfo._itemIndex], indexOfSecondCell, type)) return;
        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = ItemsArray[indexOfSecondCell];
                ItemsArray[indexOfSecondCell] = PlayerItems[_selectedItemInfo._itemIndex];
                PlayerItems[_selectedItemInfo._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = PlayerItems[indexOfSecondCell];
                PlayerItems[indexOfSecondCell] = PlayerItems[_selectedItemInfo._itemIndex];
                PlayerItems[_selectedItemInfo._itemIndex] = temp;
                break;
        }
        UIController.GetInstance().UnpinItemFromMouse();
        UIController.GetInstance().RedrawInventories();
        _selectedItemInfo = new SelectedItemInfo();
    }

    private void ApplyActiveItem()
    {
        PlayerController.GetInstance().ChangeActiveItemInHand(PlayerItems[_activePlayerItemIndex]);
        UIController.GetInstance().SelectingPlayerCell(_activePlayerItemIndex);
    }

    private void Awake()
    {
        _mainInventory.Init();
        _playerInventory.Init();
        instance = this;
    }

    private void Start()
    {
        ChangeActiveItem(true);
    }

    private void OnApplicationQuit()
    {
        ApplyItemDestructFromInventory();
    }

    private void ApplyItemDestructFromInventory()
    {
        foreach (var item in ItemsArray)
        {
            if (item == null) continue;
            item.Destruct();
        }
        foreach (var item in PlayerItems)
        {
            if (item == null) continue;
            item.Destruct();
        }
    }

    public void DropItemFromInventory()
    {
        if (!_selectedItemInfo.Equals(default(SelectedItemInfo)))
        {
            GameObject dropItem = null;
            switch (_selectedItemInfo._cellType)
            {
                case CellTypeEnum.Inventory:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)ItemsArray[_selectedItemInfo._itemIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = ItemsArray[_selectedItemInfo._itemIndex];
                    ItemsArray[_selectedItemInfo._itemIndex] = null;
                    break;
                case CellTypeEnum.Player:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)PlayerItems[_selectedItemInfo._itemIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = PlayerItems[_selectedItemInfo._itemIndex];
                    PlayerItems[_selectedItemInfo._itemIndex] = null;
                    break;
            }
            dropItem.transform.position = _hand.position;
            _selectedItemInfo = new SelectedItemInfo();
            UIController.GetInstance().UnpinItemFromMouse();
            UIController.GetInstance().RedrawInventories();
            ApplyActiveItem();
        }
    }

    public bool AddItemToMainInventory(Item item, int amount)
    {
        return _mainInventory.AddItem(new InventorySlot(item, amount));
    }

    public void RemoveItem()
    {
        if (_selectedItemInfo.Equals(default(SelectedItemInfo)))
        {
            PlayerItems[_activePlayerItemIndex] = null;
        }

        UIController.GetInstance().RedrawInventories();
    }

    public void RemoveItem(Item removeItem, int count)
    {
        _mainInventory.RemoveItem(removeItem, ref count);
        _playerInventory.RemoveItem(removeItem, ref count);
        UIController.GetInstance().RedrawInventories();
    }

    //Select item for move to another cell
    public void SelectItem(int index, CellTypeEnum type)
    {
        if (_selectedItemInfo.Equals(default(SelectedItemInfo)))
        {
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (ItemsArray[index] == null) return;
                    break;
                case CellTypeEnum.Player:
                    if (PlayerItems[index] == null) return;
                    break;
            }
            _selectedItemInfo = new SelectedItemInfo() { _itemIndex = index, _cellType = type };
        }
        else
        {
            switch (_selectedItemInfo._cellType)
            {
                case CellTypeEnum.Inventory:
                    MoveItemFromInventory(index, type);
                    break;
                case CellTypeEnum.Player:
                    MoveItemFromPlayer(index, type);
                    break;
            }
            ApplyActiveItem();
        }
    }

    public void ChangeActiveItem(bool isPositiv = true, int index = -1)
    {
        if (!_isCanChangeActiveItem) return;

        int oldActivePlayerItemIndex = _activePlayerItemIndex;
        if (index == -1)
        {
            _activePlayerItemIndex += isPositiv ? 1 : -1;
        }
        else
        {
            _activePlayerItemIndex = index;
        }
        _activePlayerItemIndex = Mathf.Clamp(_activePlayerItemIndex, 
                                             0, GlobalConstants.MAX_ITEMS_IN_PLAYER - 1);
        if(_activePlayerItemIndex != oldActivePlayerItemIndex) ApplyActiveItem();
    }
}
