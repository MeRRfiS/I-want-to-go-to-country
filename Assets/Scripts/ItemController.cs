using FMODUnity;
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
    private Item _itemObject = null;

    [Header("FOR TESTING")]
    [SerializeField] private bool _isDroped;

    private bool _isUpdating = false;
    [Header("Prefabs")]
    [SerializeField] private GameObject _objPrefab;
    private GameObject _obj;

    public event Action OnItemBroke;

    public bool IsUpdating
    {
        set => _isUpdating = value;
    }

    public Item Item
    {
        get => _itemObject;
        set => _itemObject = value;
    }

    [Obsolete]
    private void Start()
    {
        InitializeItem();
        if(!_itemObject.IsDroped)
            _itemObject.IsDroped = _isDroped;
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

    private void OnApplicationQuit()
    {
        ApplyItemDestruct();
    }

    private void ApplyItemBroke()
    {
        if (!_itemObject.IsItemCountZero()) return;

        InventoryController.GetInstance().RemoveItem();
        OnItemBroke?.Invoke();
        RuntimeManager.PlayOneShot(FMODEvents.instance.ItemBroke, transform.position);
        Destroy(gameObject);
    }

    private void ApplyItemUpdate()
    {
        if (!_isUpdating) return;

        _obj = _itemObject.Updating(_obj, _objPrefab);
    }

    public void InitializeItem()
    {
        if (_itemObject != null) return;

        _itemObject = _item.Copy();
        _itemObject.Init();
    }

    public void ApplyItemDisable()
    {
        _obj = _itemObject.StopUpdating();
    }

    public void ApplyItemDestruct()
    {
        _itemObject.Destruct();
    }
}
