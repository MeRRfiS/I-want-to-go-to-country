using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    private bool _isFruitGrow = false;
    private bool _isNeedWater = false;
    private Animator _animator;

    public bool IsFruitGrow
    {
        get => _isFruitGrow;
    }

    public bool IsNeedWater
    {
        get => _isNeedWater;
        set => _isNeedWater = value;
    }

    public void AnimatorEnabled(bool status) => _animator.enabled = status;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetAnimation()
    {
        _isFruitGrow = false;
        _animator.Play("Grow Fruit", -1, 0f);
    }

    public void FruitGrew()
    {
        _isFruitGrow = true;
    }

    public void NeedWater()
    {
        _isNeedWater = true;
        _animator.enabled = false;
    }
}
