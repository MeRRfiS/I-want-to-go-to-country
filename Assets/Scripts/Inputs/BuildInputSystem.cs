using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildInputSystem : MonoBehaviour
{
    public static bool HoldingZButton = false;
    public static bool HoldingXButton = false;

    public static event Func <bool> OnPlacingBuilding;

    private bool IsBuildingPlacing()
    {
        if (OnPlacingBuilding == null) return false;

        return (bool)OnPlacingBuilding?.Invoke();
    }

    public void RotationBuildLeft(InputAction.CallbackContext context)
    {
        if (!IsBuildingPlacing()) return;

        if (context.performed)
        {
            HoldingZButton = true;
        }
        else if (context.canceled)
        {
            HoldingZButton = false;
        }
    }

    public void RotationBuildRight(InputAction.CallbackContext context)
    {
        if (!IsBuildingPlacing()) return;

        if (context.performed)
        {
            HoldingXButton = true;
        }
        else if (context.canceled)
        {
            HoldingXButton = false;
        }
    }
}
