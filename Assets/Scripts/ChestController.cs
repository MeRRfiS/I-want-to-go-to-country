using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private Inventory _inventory;

    public Inventory Inventory
    {
        get => _inventory;
    }

    private void Awake()
    {
        if(_inventory == null)
        {
            _inventory = new ChestInventory();
        }
        _inventory.Init();
    }

    public void LoadChestInventoryToUI()
    {
        InventoryController.GetInstance().SetChestInventory(_inventory);
        UIController.GetInstance().SwitchActiveChestMenu();
    }
}
