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
    public event Action OnNewItem;
    public event Action OnChangeItem;

    public static HandsAnimationManager GetInstance() => _instance;
    public void IsChangingInst(bool status) 
    {
        if (_hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_funel_pick") && status)
        {
            Debug.Log("1");
        }

        if(_hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_instrument_idle") ||
           _hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_funel_idle") ||
           _hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_Armature_Hands_Instrument_walk") ||
           _hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_Armature_Hands_funel_walk"))
        {
            _hands.SetBool("_IsChangingInst", status);
            Debug.Log("Yes, I`m change");
        }
        else
        {
            _hands.SetBool("_IsChangingInst", false);
            Debug.Log($"No, I don't change: {_hands.GetCurrentAnimatorStateInfo(0).IsName("Hands_funel_pick")}");
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
        InventoryController.GetInstance().IsCanChangeActiveItem = false;
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
        OnNewItem?.Invoke();
    }

    public void ItemChangeStart()
    {
        InventoryController.GetInstance().IsCanChangeActiveItem = false;
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
