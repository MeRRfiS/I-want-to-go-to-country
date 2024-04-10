using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputSystem : MonoBehaviour
{
    public static Func<BlockUIEnum> BlockInputSystem;

    private bool IsBlocked(BlockUIEnum status)
    {
        if (BlockInputSystem == null) return false;

        return BlockInputSystem.Invoke() != status;
    }

    public void CloseGame(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (IsBlocked(BlockUIEnum.GameMenu)) return;
        if (UIController.GetInstance().GameMenuActiveSelf()) return;

        UIController.GetInstance().SwitchActiveGameMenu();
    }

    public void Inventory(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (IsBlocked(BlockUIEnum.InventoryMenu)) return;

        UIController.GetInstance().SwitchActiveInventoryMenu();
    }

    public void ChangeActiveItem(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() < 0)
        {
            InventoryController.GetInstance().ChangeActiveItem(isPositiv: false);
        }
        else if(context.ReadValue<float>() > 0)
        {
            InventoryController.GetInstance().ChangeActiveItem(isPositiv: true);
        }
    }

    public void ChangeActiveItemByKeyboard(InputAction.CallbackContext context) 
    {
        if (!context.started) return;

        InventoryController.GetInstance().ChangeActiveItem(index: (int)context.ReadValue<float>());
    }
}
