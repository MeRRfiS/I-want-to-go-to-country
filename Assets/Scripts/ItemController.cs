using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ItemController : MonoBehaviour
{
    private Item _item;

    [Header("Settings")]
    [SerializeField] private ItemTypeEnum _itemType;
    [SerializeField] private InstrumentTypeEnum _instrumentType;
    [SerializeField] private SeedTypeEnum _seedType;
    [SerializeField] private TreeTypeEnum _treeType;
    public int _level;
    public int _durability;

    private bool _isUpdating = false;
    private int _id;
    [Header("Prefabs")]
    [SerializeField] private GameObject _objPrefab;
    private GameObject _obj;

    public bool IsUpdating
    {
        set => _isUpdating = value;
    }

    public Item Item
    {
        get => _item;
        set => _item = value;
    }

    [Obsolete]
    private void Start()
    {
        if (_item != null) return;

        switch (_itemType)
        {
            case ItemTypeEnum.None:
                break;
            case ItemTypeEnum.Instrument:
                _item = Instrument.CreateInstrument(_instrumentType, _level, _durability);
                break;
            case ItemTypeEnum.Seed:
                _item = new Seed(_seedType);
                break;
            case ItemTypeEnum.Tree:
                _item = new Tree(_treeType);
                break;
            case ItemTypeEnum.Fertilizers:
                _item = new Fertilizers(_level);
                break;
            default:
                _item = new Item();
                _item.Id = (int)ItemIdsEnum.Harvest_Default;
                break;
        }
        _item.Type = _itemType;
    }

    private void Update()
    {
        ApplyItemUpdate();
        ApplyItemBroke();
    }

    private void OnDestroy()
    {
        ApplyItemDisable();
    }

    private void ApplyItemBroke()
    {
        if (!_item.IsItemCountZero()) return;

        InventoryController.GetInstance().RemoveItem();
        Destroy(gameObject);
    }

    private void ApplyItemUpdate()
    {
        if (!_isUpdating) return;

        _obj = _item.Updating(_obj, _objPrefab);
    }

    public void ApplyItemDisable()
    {
        _obj = _item.StopUpdating();
    }
}
