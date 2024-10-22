using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private static InventoryController instance;

    private bool _isCanChangeActiveItem = true;
    private bool _isOpenUI = false;
    private MovedItemsModel _movedItemsModel;
    private Inventory _chestInventory;

    [Header("Inventory")]
    [SerializeField] private Inventory _mainInventory;
    [SerializeField] private Inventory _playerInventory;

    [Header("Components")]
    [SerializeField] private Transform _hand;

    public int ActivePlayerItemIndex { get; private set; } = 0;
    public static InventoryController GetInstance() => instance;

    public bool IsCanChangeActiveItem
    {
        set => _isCanChangeActiveItem = value;
    }

    public bool IsOpenUI
    {
        set => _isOpenUI = value;
    }

    public Item[] ItemsArray
    {
        get => _mainInventory.Container;
    }

    public Item[] PlayerItems
    {
        get => _playerInventory.Container;
    }

    public Item[] ChestItems
    {
        get => _chestInventory.Container;
    }

    public Inventory ChestInventory
    {
        get => _chestInventory;
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
    }

    private void ApplyActiveItem()
    {
        PlayerController.GetInstance().ChangeActiveItemInHand(PlayerItems[ActivePlayerItemIndex]);
        UIController.GetInstance().SelectingPlayerCell(ActivePlayerItemIndex);
        UIController.GetInstance().StopProgressBar();
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

            ItemController dropItem = null;

            void Drop(Item[] from)
            {
                dropItem = Instantiate(from[_movedItemsModel.FirstIndex].Object);
                dropItem.Item = from[_movedItemsModel.FirstIndex];
                from[_movedItemsModel.FirstIndex] = null;
            }

            switch (_movedItemsModel.FirstCellTypeEnum)
            {
                case CellTypeEnum.Inventory:
                    Drop(ItemsArray);
                    break;
                case CellTypeEnum.Player:
                    Drop(PlayerItems);
                    break;
                case CellTypeEnum.Chest:
                    Drop(ChestItems);
                    break;
            }
            dropItem.Item.IsDroped = true;
            dropItem.transform.position = _hand.position;
            _movedItemsModel = null;
            UIController.GetInstance().UnpinItemFromMouse();
            UIController.GetInstance().RedrawInventories();
            ApplyActiveItem();
        }
    }

    public void DropItemFromInventory(Item item, int amount = 0, Vector3? dropPosition = null)
    {
        ItemController dropItem = Instantiate(item.Object);
        dropItem.Item = item;
        dropItem.Item.Amount = amount;
        dropItem.Item.IsDroped = true;
        Debug.Log(dropPosition.HasValue);
        if (dropPosition.HasValue)
            dropItem.transform.position = dropPosition.GetValueOrDefault();
        else
            dropItem.transform.position = _hand.position;
    }

    public bool AddItemToMainInventory(Item item, int amount, Vector3? possibleDropPosition = null)
    {
        return _mainInventory.AddItem(new InventorySlot(item, amount), possibleDropPosition);
    }

    public void RemoveItem()
    {
        if (_movedItemsModel == null)
        {
            PlayerItems[ActivePlayerItemIndex] = null;
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
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (ItemsArray[index] == null) return;
                    _movedItemsModel = new MovedItemsModel();
                    _movedItemsModel.FromInventory = _mainInventory;
                    break;
                case CellTypeEnum.Player:
                    if (PlayerItems[index] == null) return;
                    _movedItemsModel = new MovedItemsModel();
                    _movedItemsModel.FromInventory = _playerInventory;
                    break;
                case CellTypeEnum.Chest:
                    if (ChestItems[index] == null) return;
                    _movedItemsModel = new MovedItemsModel();
                    _movedItemsModel.FromInventory = _chestInventory;
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
                case CellTypeEnum.Chest:
                    _movedItemsModel.ToInventory = _chestInventory;
                    break;
            }
            _movedItemsModel.SecondCellTypeEnum = type;
            _movedItemsModel.SecondIndex = index;
            MoveItemToOtherCell();

            bool activeItemIsCarry = ActivePlayerItemIndex == _movedItemsModel.FirstIndex &&
                                    _movedItemsModel.FirstCellTypeEnum == CellTypeEnum.Player;

            switch (type)
            {
                case CellTypeEnum.Inventory:
                case CellTypeEnum.Chest:
                    if (activeItemIsCarry) ApplyActiveItem();
                    break;
                case CellTypeEnum.Player:
                    if(activeItemIsCarry || ActivePlayerItemIndex == _movedItemsModel.SecondIndex)
                    {
                        ApplyActiveItem();
                    }
                    break;
            }

            _movedItemsModel = null;
        }
    }

    public void ChangeActiveItem(bool isPositiv = true, int index = -1)
    {
        if (_isOpenUI) return;
        if (!_isCanChangeActiveItem) return;

        int oldActivePlayerItemIndex = ActivePlayerItemIndex;
        if (index == -1)
        {
            ActivePlayerItemIndex += isPositiv ? 1 : -1;
        }
        else
        {
            ActivePlayerItemIndex = index;
        }
        ActivePlayerItemIndex = Mathf.Clamp(ActivePlayerItemIndex, 
                                             0, MechConstants.MAX_ITEMS_IN_PLAYER - 1);

        if(ActivePlayerItemIndex != oldActivePlayerItemIndex) ApplyActiveItem();
    }

    public void SetChestInventory(Inventory chestInventory)
    {
        _chestInventory = chestInventory;
    }
}
