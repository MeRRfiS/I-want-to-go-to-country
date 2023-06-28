using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InstrumentController : MonoBehaviour
{
    private Instrument instrument;

    [Header("Settings")]
    public int _level;
    public int _durability;

    [Header("Prefabs")]
    [SerializeField] private GameObject _patchPrefab;
    private GameObject _patch;

    public Instrument Instrument
    {
        get => instrument;
    }

    [Obsolete]
    private void Start()
    {
        instrument = new Hoe();
    }

    private void Update()
    {
        _patch = instrument.CreateObj(_patch, _patchPrefab);
    }

    private void OnDisable()
    {
        _patch = instrument.DestroyObj(_patch);
    }

    public void SetValue(InstrumentModel instrumentModel)
    {
        switch (instrumentModel.InstrumentType)
        {
            case InstrumentTypeEnum.None:
                break;
            case InstrumentTypeEnum.Hoe:
                instrument = new Hoe();
                break;
            case InstrumentTypeEnum.Axe:
                break;
            case InstrumentTypeEnum.Funnel:
                break;
            default:
                break;
        }
        _level = instrumentModel.Level;
        _durability = instrumentModel.Durability;
    }
}
