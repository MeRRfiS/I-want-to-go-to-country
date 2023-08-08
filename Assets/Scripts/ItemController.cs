using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ItemController : MonoBehaviour
{
    private Item item;

    [Header("Settings")]
    [SerializeField] private ItemTypeEnum _itemType;
    [SerializeField] private InstrumentTypeEnum _instrumentType;
    [SerializeField] private SeedTypeEnum _seedType;
    [SerializeField] private TreeTypeEnum _treeType;
    public int _level;
    public int _durability;
    private int _count = 1;

    private bool _isUpdating = false;
    private int _id;
    [Header("Prefabs")]
    [SerializeField] private GameObject _objPrefab;
    private GameObject _obj;

    public bool IsUpdating
    {
        set => _isUpdating = value;
    }

    public int Count
    {
        set => _count = value;
    }

    public Item Item
    {
        get => item;
    }

    [Obsolete]
    private void Start()
    {
        switch (_itemType)
        {
            case ItemTypeEnum.None:
                break;
            case ItemTypeEnum.Instrument:
                item = Instrument.CreateInstrument(_instrumentType, _level, _durability);
                break;
            case ItemTypeEnum.Seed:
                item = new Seed(_seedType);
                break;
            case ItemTypeEnum.Tree:
                item = new Tree(_treeType);
                break;
            case ItemTypeEnum.Fertilizers:
                item = new Fertilizers(_level);
                break;
            default:
                item = new Item();
                item.Id = (int)ItemIdsEnum.Harvest_Default;
                break;
        }
        item.Type = _itemType;
        item.Count = _count;
        //_isUpdating = false;
    }

    private void Update()
    {
        ApplyItemUpdate();
    }

    private void OnDestroy()
    {
        ApplyItemDisable();
    }

    private void ApplyItemUpdate()
    {
        if (!_isUpdating) return;

        _obj = item.Updating(_obj, _objPrefab);
    }

    public void ApplyItemDisable()
    {
        _obj = item.StopUpdating();
    }
}
