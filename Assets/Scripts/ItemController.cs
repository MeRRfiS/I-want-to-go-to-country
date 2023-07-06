using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemController : MonoBehaviour
{
    private Item item;

    [Header("Settings")]
    public InstrumentTypeEnum instrumentType;
    public PlantTypeEnum seedType;
    public int _level;
    public int _durability;

    [Header("Prefabs")]
    [SerializeField] private GameObject _patchPrefab;
    private GameObject _patch;

    public Item Item
    {
        get => item;
    }

    [Obsolete]
    private void Start()
    {
        switch (instrumentType)
        {
            case InstrumentTypeEnum.None:
                item = new Seed(seedType);
                break;
            case InstrumentTypeEnum.Hoe:
                item = new Hoe(_level, _durability);
                break;
            case InstrumentTypeEnum.Axe:
                break;
            case InstrumentTypeEnum.Funnel:
                item = new Funnel(_level, _durability);
                break;
            default:
                break;
        }
            
    }

    private void Update()
    {
        ApplyHoeUpdate();
    }

    private void OnDisable()
    {
        ApplyHoeDisable();
    }

    private void ApplyHoeUpdate()
    {
        if (!item.GetType().Equals(typeof(Hoe))) return;

        Hoe hoe = (Hoe)item;
        _patch = hoe.CreatePatch(_patch, _patchPrefab);
    }

    private void ApplyHoeDisable()
    {
        if (!item.GetType().Equals(typeof(Hoe))) return;

        Hoe hoe = (Hoe)item;
        _patch = hoe.DestroyPatch(_patch);
    }
}
