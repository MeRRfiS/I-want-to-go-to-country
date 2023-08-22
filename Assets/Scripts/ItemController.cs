using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ItemController : MonoBehaviour
{
    [SerializeField] private Item _item;

    private bool _isUpdating = false;
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
        InitializeItem();
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

    public void InitializeItem()
    {
        _item.Init();
    }

    public void ApplyItemDisable()
    {
        _obj = _item.StopUpdating();
    }
}
