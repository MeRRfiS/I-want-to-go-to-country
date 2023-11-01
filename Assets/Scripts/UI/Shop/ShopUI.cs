using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    private bool BlockerInputSystem() => true;

    private void OnEnable()
    {
        PlayerInputSystem.BlockInputSystem += BlockerInputSystem;
        UIInputSystem.BlockInputSystem += BlockerInputSystem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.BlockInputSystem -= BlockerInputSystem;
        UIInputSystem.BlockInputSystem -= BlockerInputSystem;
    }
}
