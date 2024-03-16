using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FruitManager : MonoBehaviour
{
    private bool _isFruitGrow = false;
    private bool _isNeedWater = false;

    [SerializeField] private UnityEvent TreeFruitsGrew;
    [SerializeField] private UnityEvent TreeFruitsNeedWater;

    public bool IsFruitGrow
    {
        get => _isFruitGrow;
    }

    public bool IsNeedWater
    {
        get => _isNeedWater;
        set => _isNeedWater = value;
    }

    public void FruitsGrew()
    {
        TreeFruitsGrew?.Invoke();
    }

    public void FruitsNeedWater()
    {
        TreeFruitsNeedWater?.Invoke();
    }
}
