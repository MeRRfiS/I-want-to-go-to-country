using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    private bool BlockerPlayerInputSystem() => true;
    private BlockUIEnum BlockerUIInputSystem() => BlockUIEnum.ChestMenu;

    private void OnEnable()
    {
        PlayerInputSystem.BlockInputSystem += BlockerPlayerInputSystem;
        UIInputSystem.BlockInputSystem += BlockerUIInputSystem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.BlockInputSystem -= BlockerPlayerInputSystem;
        UIInputSystem.BlockInputSystem -= BlockerUIInputSystem;
    }
}
