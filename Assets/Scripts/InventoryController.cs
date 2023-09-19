using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private static InventoryController instance;

    private bool _isCanChangeActiveItem = true;
    private int _activePlayerItemIndex = 0;
    private MovedItemsModel _movedItemsModel;

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

    private bool ToStackItems()
    {
        if (_movedItemsModel.FirstCellTypeEnum == _movedItemsModel.SecondCellTypeEnum &&
            _movedItemsModel.FirstIndex == _movedItemsModel.SecondIndex) return false;
        Item selectedItem = _movedItemsModel.FromInventory.Container[_movedItemsModel.FirstIndex];
        if (selectedItem == null) return false;
        if (selectedItem is Instrument || selectedItem is Fertilizers) return false;

        bool result = _movedItemsModel.FromInventory.StackItem(_movedItemsModel.FirstIndex,
                                                                  _movedItemsModel.SecondIndex,
                                                                  _movedItemsModel.ToInventory);

        return result;
    }

    private void MoveItemToOtherCell()
    {
        if (_movedItemsModel.FromInventory.Container[_movedItemsModel.FirstIndex] == null) return;

        if (ToStackItems())
        {
            if (_movedItemsModel.FromInventory.Container[_movedItemsModel.FirstIndex] == null)
            {
                _movedItemsModel = null;
            }

            return;
        }
        _movedItemsModel.FromInventory.MoveItem(_movedItemsModel.FirstIndex,
                                                _movedItemsModel.SecondIndex,
                                                _movedItemsModel.ToInventory);

        UIController.GetInstance().UnpinItemFromMouse();
        UIController.GetInstance().RedrawInventories();
        _movedItemsModel = null;
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
        if (_movedItemsModel != null)
        {
            GameObject dropItem = null;
            switch (_movedItemsModel.FirstCellTypeEnum)
            {
                case CellTypeEnum.Inventory:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)ItemsArray[_movedItemsModel.FirstIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = ItemsArray[_movedItemsModel.FirstIndex];
                    ItemsArray[_movedItemsModel.FirstIndex] = null;
                    break;
                case CellTypeEnum.Player:
                    dropItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS +
                                           (ItemIdsEnum)PlayerItems[_movedItemsModel.FirstIndex]._id));
                    dropItem.GetComponent<ItemController>().Item = PlayerItems[_movedItemsModel.FirstIndex];
                    PlayerItems[_movedItemsModel.FirstIndex] = null;
                    break;
            }
            dropItem.transform.position = _hand.position;
            _movedItemsModel = null;
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
        if (_movedItemsModel == null)
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
        if (_movedItemsModel == null)
        {
            _movedItemsModel = new MovedItemsModel();
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (ItemsArray[index] == null) return;
                    _movedItemsModel.FromInventory = _mainInventory;
                    break;
                case CellTypeEnum.Player:
                    if (PlayerItems[index] == null) return;
                    _movedItemsModel.FromInventory = _playerInventory;
                    break;
            }
            _movedItemsModel.FirstCellTypeEnum = type;
            _movedItemsModel.FirstIndex = index;
        }
        else
        {
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    _movedItemsModel.ToInventory = _mainInventory;
                    break;
                case CellTypeEnum.Player:
                    _movedItemsModel.ToInventory = _playerInventory;
                    break;
            }
            _movedItemsModel.SecondCellTypeEnum = type;
            _movedItemsModel.SecondIndex = index;
            MoveItemToOtherCell();
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
