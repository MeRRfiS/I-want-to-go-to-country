using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator _hands;

    public void HideInstrument()
    {
        _hands.SetBool("_IsChangingInst", false);
        if (PlayerController.GetInstance().HeldItem != null)
        {
            if (PlayerController.GetInstance().HeldItem.Item is Funnel)
            {
                _hands.SetBool("_IsHoldFunnel", false);
            }
            else
            {
                _hands.SetBool("_IsHoldInst", false);
            }
        }
    }

    public void ClearHand()
    {
        PlayerController.GetInstance().ClearHeldInstrument();
    }
}
