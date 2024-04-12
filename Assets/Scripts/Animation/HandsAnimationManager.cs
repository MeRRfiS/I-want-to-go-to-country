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
    public void IsChangingInst(bool status) 
    {
        if(_hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_instrument_idle") ||
           _hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_funel_idle"))
        {
            _hands.SetBool("_IsChangingInst", status);
        }
        else
        {
            _hands.SetBool("_IsChangingInst", false);
        }
    } 
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
2        if (!IsChangeItem)
        {
            IsChangingInst(false);
        }

        IsHoldFunnel(false);
        IsHoldInst(false);

        InventoryController.GetInstance().IsCanChangeActiveItem = false;
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

    public void UnblockChangeItem()
    {
        InventoryController.GetInstance().IsCanChangeActiveItem = true;
    }
}
