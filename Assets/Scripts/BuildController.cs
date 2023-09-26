using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CraftsCollection _craftCollection;
    [SerializeField] private UpgradesCollection _upgradeCollection;

    private int _level = 1;

    public int Level
    {
        get => _level;
    }

    public float DecreaseCountMultipler
    {
        get
        {
            switch (_level)
            {
                case 1: return 0;
                case 2: return MechConstants.DICREASE_MULTIPLER_FOR_CRAFT_LEVEL_2;
                case 3: return MechConstants.DICREASE_MULTIPLER_FOR_CRAFT_LEVEL_3;
                default: return 0;
            }
        }
    }

    public UpgradesCollection UpgradeCollection
    {
        get => _upgradeCollection;
    }

    private bool IsHasNeededItem(List<Item> items, InformationModel infoModel, List<int> amount = null)
    {
        for (int i = 0; i < infoModel._neededItems.Count; i++)
        {
            NeededItem neededItem = infoModel._neededItems[i];

            List<Item> neededItems = items.Where(item =>
            {
                if (item == null) return false;
                if (item._id != neededItem._item._id) return false;

                return true;
            }).ToList();
            if (neededItems.Count == 0) return false;

            int itemsAmount = neededItems.Select(item => item.Amount).Sum();
            if (itemsAmount < (amount == null ? neededItem._amount : amount[i])) return false;
        }

        return true;
    }

    public void LoadCraftsCollectionToUI()
    {
        UIController.GetInstance().RedrawCraftMenu(_craftCollection._craftedItemsList, this);
        UIController.GetInstance().SwitchActiveCraftMenu();
    }

    public bool IsCanDoSomething(InformationModel infoModel, List<int> amount = null)
    {
        Item[] inventoryItems = InventoryController.GetInstance().ItemsArray;
        Item[] playerItems = InventoryController.GetInstance().PlayerItems;
        List<Item> allItems = new List<Item>();
        foreach (var item in inventoryItems)
        {
            allItems.Add(item);
        }
        foreach (var item in playerItems)
        {
            allItems.Add(item);
        }

        bool isHasItem = IsHasNeededItem(allItems, infoModel, amount);

        return isHasItem;
    }

    public bool UpgradeBuilding(int newLevel)
    {
        UpgradeModel upgradeModel = _upgradeCollection._upgrades[newLevel - 1];
        bool isCanCraft = IsCanDoSomething(upgradeModel);
        if (isCanCraft && PlayerController.GetInstance().Money >= upgradeModel._price)
        {
            foreach (var upgradedItem in upgradeModel._neededItems)
            {
                InventoryController.GetInstance().RemoveItem(upgradedItem._item, upgradedItem._amount);
            }
            UIController.GetInstance().RedrawInventories();
            PlayerController.GetInstance().Money -= upgradeModel._price;
            _level = newLevel;

            return true;
        }

        return false;
    }
}
