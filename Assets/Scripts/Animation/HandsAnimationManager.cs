using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator _hands;

    private static HandsAnimationManager _instance;

    public bool IsChangeItem { private get; set; }

    public event Action OnHideItem;
    public event Action OnChangeItem;

    public static HandsAnimationManager GetInstance() => _instance;
    public void IsChangingInst(bool status) => _hands.SetBool("_IsChangingInst", status);
    public void IsHoldFunnel(bool status) => _hands.SetBool("_IsHoldFunnel", status);
    public void IsHoldInst(bool status) => _hands.SetBool("_IsHoldInst", status);
    public void IsMoving(bool status) => _hands.SetBool("_IsMoving", status);

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void InstrumentHideStart()
    {
        if (!IsChangeItem)
        {
            IsChangingInst(false);
        }

        IsHoldFunnel(false);
        IsHoldInst(false);
    }

    public void InstrumentHideFinish()
    {
        OnHideItem?.Invoke();
    }

    public void ItemChangeStart()
    {
        OnChangeItem?.Invoke();
    }

    public void ItemChangeFinish()
    {
        IsChangingInst(false);
    }
}
