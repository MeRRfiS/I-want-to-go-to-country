using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsAnimationManager : MonoBehaviour
{
    private const string CHANGING_ITEM = "_IsChangingItem";
    private const string HOLD_INTRUMENT = "_IsHoldInst";
    private const string HOLD_FUNNEL = "_IsHoldFunnel";
    private const string HOLD_STUF = "_IsHoldStuf";
    private const string MOVING = "_IsMoving";

    [SerializeField] private Animator _hands;

    private static HandsAnimationManager _instance;

    public bool IsChangeItem { private get; set; }

    public event Action OnHideItem;
    public event Action OnNewItem;
    public event Action OnChangeItem;

    public static HandsAnimationManager GetInstance() => _instance;
    public void IsChangingItem(bool status) => _hands.SetBool(CHANGING_ITEM, status);
    public void IsHoldInst(bool status) => _hands.SetBool(HOLD_INTRUMENT, status);
    public void IsHoldFunnel(bool status) => _hands.SetBool(HOLD_FUNNEL, status);
    public void IsHoldStaf(bool status) => _hands.SetBool(HOLD_STUF, status);
    public void IsMoving(bool status) => _hands.SetBool(MOVING, status);

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
            IsChangingItem(false);
        }

        IsHoldInst(false);
        IsHoldFunnel(false);
        IsHoldStaf(false);
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
        IsChangingItem(false);
    }

    public void UnblockChangeItem()
    {
        InventoryController.GetInstance().IsCanChangeActiveItem = true;
    }
}
