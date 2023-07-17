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
    public ItemTypeEnum itemType;
    public SeedTypeEnum seedType;
    public TreeTypeEnum treeType;
    public int _level;
    public int _durability;

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
        switch (itemType)
        {
            case ItemTypeEnum.None:
                break;
            case ItemTypeEnum.Hoe:
                item = new Hoe(_level, _durability);
                break;
            case ItemTypeEnum.Axe:
                item = new Axe(_level, _durability);
                break;
            case ItemTypeEnum.Funnel:
                item = new Funnel(_level, _durability);
                break;
            case ItemTypeEnum.Seed:
                item = new Seed(seedType);
                break;
            case ItemTypeEnum.Tree:
                item = new Tree(treeType);
                break;
            default:
                break;
        }
            
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
