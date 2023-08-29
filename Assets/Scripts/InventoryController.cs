using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private struct SelectedItem
    {
        public int _itemIndex;
        public CellTypeEnum _cellType;
    }

    private static InventoryController instance;

    private bool _isCanChangeActiveItem = true;
    private int _activePlayerItemIndex = 0;
    private SelectedItem _selectedItem;

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

    private void MoveItemFromInventory(int indexOfSecondCell, CellTypeEnum type)
    {
        if (_mainInventory.Container[_selectedItem._itemIndex] == null) return;

        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = _mainInventory.Container[indexOfSecondCell];
                _mainInventory.Container[indexOfSecondCell] = _mainInventory.Container[_selectedItem._itemIndex];
                _mainInventory.Container[_selectedItem._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = _playerInventory.Container[indexOfSecondCell];
                _playerInventory.Container[indexOfSecondCell] = _mainInventory.Container[_selectedItem._itemIndex];
                _mainInventory.Container[_selectedItem._itemIndex] = temp;
                break;
        }

        UIController.GetInstance().RedrawInventories();
    }

    private void MoveItemFromPlayer(int indexOfSecondCell, CellTypeEnum type)
    {
        if (_playerInventory.Container[_selectedItem._itemIndex] == null) return;

        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = _mainInventory.Container[indexOfSecondCell];
                _mainInventory.Container[indexOfSecondCell] = _playerInventory.Container[_selectedItem._itemIndex];
                _playerInventory.Container[_selectedItem._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = _playerInventory.Container[indexOfSecondCell];
                _playerInventory.Container[indexOfSecondCell] = _playerInventory.Container[_selectedItem._itemIndex];
                _playerInventory.Container[_selectedItem._itemIndex] = temp;
                break;
        }

        UIController.GetInstance().RedrawInventories();
    }

    private void ApplyActiveItem()
    {
        PlayerController.GetInstance().ChangeActiveItemInHand(_playerInventory.Container[_activePlayerItemIndex]);
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
        if (!_selectedItem.Equals(default(SelectedItem)))
        {
            GameObject dropItem = null;
            switch (_selectedItem._cellType)
            {
                case CellTypeEnum.Inventory:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)_mainInventory.Container[_selectedItem._itemIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = _mainInventory.Container[_selectedItem._itemIndex];
                    _mainInventory.Container[_selectedItem._itemIndex] = null;
                    break;
                case CellTypeEnum.Player:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)_playerInventory.Container[_selectedItem._itemIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = _playerInventory.Container[_selectedItem._itemIndex];
                    _playerInventory.Container[_selectedItem._itemIndex] = null;
                    break;
            }
            dropItem.transform.position = _hand.position;
            _selectedItem = new SelectedItem();
            UIController.GetInstance().PinUpItemToMouse();
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
        if (_selectedItem.Equals(default(SelectedItem)))
        {
            _playerInventory.Container[_activePlayerItemIndex] = null;
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
        if (_selectedItem.Equals(default(SelectedItem)))
        {
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (_mainInventory.Container[index] == null) return;
                    break;
                case CellTypeEnum.Player:
                    if (_playerInventory.Container[index] == null) return;
                    break;
            }
            _selectedItem = new SelectedItem() { _itemIndex = index, _cellType = type };
        }
        else
        {
            switch (_selectedItem._cellType)
            {
                case CellTypeEnum.Inventory:
                    MoveItemFromInventory(index, type);
                    break;
                case CellTypeEnum.Player:
                    MoveItemFromPlayer(index, type);
                    break;
            }
            _selectedItem = new SelectedItem();
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
