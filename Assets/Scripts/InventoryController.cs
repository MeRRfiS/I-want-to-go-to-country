using System.Threading;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private struct SelectedItem
    {
        public int _itemIndex;
        public CellTypeEnum _cellType;
    }

    private static InventoryController instance;

    private int _activePlayerItemIndex = 0;
    private SelectedItem _selectedItem;
    private Item[] _playerItems = new Item[GlobalConstants.MAX_ITEMS_IN_PLAYER];
    private Item[] _itemsArray = new Item[GlobalConstants.MAX_ITEMS_IN_INVENTORY];

    [Header("Components")]
    [SerializeField] private Transform _hand;

    public static InventoryController GetInstance() => instance;

    public Item[] ItemsArray
    {
        get => _itemsArray;
    }

    public Item[] PlayerItems
    {
        get => _playerItems;
    }

    private void MoveItemFromInventory(int indexOfSecondCell, CellTypeEnum type)
    {
        if (_itemsArray[_selectedItem._itemIndex] == null) return;

        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = _itemsArray[indexOfSecondCell];
                _itemsArray[indexOfSecondCell] = _itemsArray[_selectedItem._itemIndex];
                _itemsArray[_selectedItem._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = _playerItems[indexOfSecondCell];
                _playerItems[indexOfSecondCell] = _itemsArray[_selectedItem._itemIndex];
                _itemsArray[_selectedItem._itemIndex] = temp;
                break;
        }

        UIController.GetInstance().RedrawInventories();
    }

    private void MoveItemFromPlayer(int indexOfSecondCell, CellTypeEnum type)
    {
        if (_playerItems[_selectedItem._itemIndex] == null) return;

        Item temp;
        switch (type)
        {
            case CellTypeEnum.Inventory:
                temp = _itemsArray[indexOfSecondCell];
                _itemsArray[indexOfSecondCell] = _playerItems[_selectedItem._itemIndex];
                _playerItems[_selectedItem._itemIndex] = temp;
                break;
            case CellTypeEnum.Player:
                temp = _playerItems[indexOfSecondCell];
                _playerItems[indexOfSecondCell] = _playerItems[_selectedItem._itemIndex];
                _playerItems[_selectedItem._itemIndex] = temp;
                break;
        }

        UIController.GetInstance().RedrawInventories();
    }

    private bool GetItemIndex(Item item, out int index)
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (_itemsArray[i] == null) continue;
            if (_itemsArray[i].Id == item.Id && _itemsArray[i].Count != 100)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool GetEmptyCell(out int index)
    {
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (_itemsArray[i] == null)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private bool AddNewValuesToArray(Item item, int value)
    {
        while (value > 0)
        {
            int index;
            if (!GetEmptyCell(out index)) return false;

            _itemsArray[index] = item.Copy();
            _itemsArray[index].Count = value > GlobalConstants.MAX_ITEM_IN_CELL ?
                                          GlobalConstants.MAX_ITEM_IN_CELL : value;
            value -= 100;
            //ToDo: Make throwing items if inventory full
        }

        return true;
    }

    private void ApplyActiveItem()
    {
        PlayerController.GetInstance().ChangeActiveItemInHand(_playerItems[_activePlayerItemIndex]);
        UIController.GetInstance().SelectingPlayerCell(_activePlayerItemIndex);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeActiveItem(true);
    }

    private void Update()
    {
        //ApplyActiveItem();
    }

    public void RemoveItem(CellTypeEnum type = CellTypeEnum.None)
    {
        if (_selectedItem.Equals(default(SelectedItem))) 
        {
            _playerItems[_activePlayerItemIndex] = null;
        }

        UIController.GetInstance().RedrawInventories();
    }

    public void DropItem()
    {
        if (!_selectedItem.Equals(default(SelectedItem)))
        {
            GameObject dropItem = null;
            switch (_selectedItem._cellType)
            {
                case CellTypeEnum.Inventory:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)_itemsArray[_selectedItem._itemIndex].Id));
                    _itemsArray[_selectedItem._itemIndex] = null;
                    break;
                case CellTypeEnum.Player:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)_playerItems[_selectedItem._itemIndex].Id));
                    _playerItems[_selectedItem._itemIndex] = null;
                    break;
            }
            dropItem.transform.position = _hand.position;
            _selectedItem = new SelectedItem();
            UIController.GetInstance().PinUpItemToMouse();
            UIController.GetInstance().RedrawInventories();
        }
    }

    public bool AddItem(Item item, int value)
    {
        int index;
        switch (item.Type)
        {
            case ItemTypeEnum.None:
                return false;
            case ItemTypeEnum.Instrument:
            case ItemTypeEnum.Fertilizers:
                if (!GetEmptyCell(out index)) return false;

                _itemsArray[index] = item.Copy();
                break;
            case ItemTypeEnum.Seed:
            case ItemTypeEnum.Tree:
            case ItemTypeEnum.Harvest:
                if (GetItemIndex(item, out index))
                {
                    if (_itemsArray[index].Count + value > GlobalConstants.MAX_ITEM_IN_CELL)
                    {
                        value -= GlobalConstants.MAX_ITEM_IN_CELL - _itemsArray[index].Count;
                        _itemsArray[index].Count = GlobalConstants.MAX_ITEM_IN_CELL;
                        if(!AddNewValuesToArray(item, value)) return false;
                    }
                    else
                    {
                        _itemsArray[index].Count += value;
                    }
                }
                else
                {
                    if (!AddNewValuesToArray(item, value)) return false;
                }
                break;
        }

        return true;
    }

    //Select item for move to another cell
    public void SelectItem(int index, CellTypeEnum type)
    {
        if (_selectedItem.Equals(default(SelectedItem)))
        {
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (_itemsArray[index] == null) return;
                    break;
                case CellTypeEnum.Player:
                    if (_playerItems[index] == null) return;
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
        if(index == -1)
        {
            _activePlayerItemIndex += isPositiv ? 1 : -1;
        }
        else
        {
            _activePlayerItemIndex = index;
        }
        _activePlayerItemIndex = Mathf.Clamp(_activePlayerItemIndex, 
                                             0, GlobalConstants.MAX_ITEMS_IN_PLAYER - 1);
        ApplyActiveItem();
    }
}
