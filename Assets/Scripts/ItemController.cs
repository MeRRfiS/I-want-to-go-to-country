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
    public int _count;

    private int _id;
    [Header("Prefabs")]
    [SerializeField] private GameObject _objPrefab;
    private GameObject _obj;

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
            default:
                break;
        }
        item.Type = _itemType;
        item.Count = _count;
        GetComponent<ItemController>().enabled = false;
    }

    private void Update()
    {
        ApplyItemUpdate();
    }

    private void OnDisable()
    {
        ApplyItemDisable();
    }

    private void ApplyItemUpdate()
    {
        _obj = item.Updating(_obj, _objPrefab);
    }

    private void ApplyItemDisable()
    {
        _obj = item.StopUpdating();
    }
}
