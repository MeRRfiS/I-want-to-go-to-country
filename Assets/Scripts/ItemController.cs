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
        if (instrumentType == InstrumentTypeEnum.Hoe)
            item = new Hoe(_level, _durability);
        else
            item = new Seed(seedType);
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
