using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputSystem : MonoBehaviour
{
    public void CloseGame(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Application.Quit();
    }
}
