using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<Transform> _cells;

    [Header("Items")]
    [SerializeField] private Inventory _inventory;

    public List<Transform> Cells
    {
        get => _cells;
    }

    private void OnEnable()
    {
        if(_inventory == null || _inventory is ChestInventory) 
        {
            _inventory = InventoryController.GetInstance().ChestInventory;
            RedrawInventory();
        }
    }

    public void RedrawInventory()
    {
        Item[] items = _inventory.Container;
        for (int i = 0; i < items.Length; i++)
        {
            InventoryCellHandler cell = _cells[i].GetComponent<InventoryCellHandler>();
            Image image = cell.ItemIcon;
            TextMeshProUGUI text = cell.TextCount;
            Slider slider = cell.SliderDurability;
            Slider waterSlider = cell.WaterValueSlider;
            if (items[i] == null)
            {
                image.sprite = null;
                image.gameObject.SetActive(false);
                text.text = String.Empty;
                text.gameObject.SetActive(false);
                slider.gameObject.SetActive(false);
                waterSlider.gameObject.SetActive(false);
                continue;
            }

            image.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)items[i]._id);
            image.gameObject.SetActive(true);
            waterSlider.gameObject.SetActive(false);

            switch (items[i]._type)
            {
                case ItemTypeEnum.Instrument:
                    Instrument instrument = items[i] as Instrument;
                    if (instrument is Funnel)
                    {
                        Funnel funnel = items[i] as Funnel;
                        waterSlider.maxValue = funnel._maxUsings;
                        waterSlider.value = funnel.Usings;
                        waterSlider.gameObject.SetActive(true);
                    }
                    slider.maxValue = instrument._maxDurability;
                    slider.value = instrument.Durability;
                    slider.gameObject.SetActive(true);
                    text.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Fertilizers:
                    slider.maxValue = MechConstants.MAX_USING_OF_FERTILIZER;
                    slider.value = (items[i] as Fertilizers).Usings;
                    slider.gameObject.SetActive(true);
                    text.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Seed:
                case ItemTypeEnum.Tree:
                case ItemTypeEnum.Harvest:
                    text.text = items[i].Amount.ToString();
                    text.gameObject.SetActive(true);
                    slider.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Building:
                    text.gameObject.SetActive(false);
                    slider.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
