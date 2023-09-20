using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinUpItemInfo : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _waterSlider;

    private InventoryCellHandler _cell;

    public bool IsItemPin() => _cell != null;

    public void PinUpItem(InventoryCellHandler cell)
    {
        _cell = cell;
        _image.sprite = _cell.ItemIcon.sprite;
        _image.gameObject.SetActive(_cell.ItemIcon.gameObject.activeSelf);
        _text.text = _cell.TextCount.text;
        _text.gameObject.SetActive(_cell.TextCount.gameObject.activeSelf);
        _slider.maxValue = _cell.SliderDurability.maxValue;
        _slider.value = _cell.SliderDurability.value;
        _slider.gameObject.SetActive(_cell.SliderDurability.gameObject.activeSelf);
        _waterSlider.maxValue = _cell.WaterValueSlider.maxValue;
        _waterSlider.value = _cell.WaterValueSlider.value;
        _waterSlider.gameObject.SetActive(_cell.WaterValueSlider.gameObject.activeSelf);

        _cell.ItemIcon.gameObject.SetActive(false);
        _cell.TextCount.gameObject.SetActive(false);
        _cell.SliderDurability.gameObject.SetActive(false);
        _cell.WaterValueSlider.gameObject.SetActive(false);
    }

    public void UpdatePinUpItemInformation()
    {
        switch (_cell.Type)
        {
            case CellTypeEnum.Inventory:
                _text.text = InventoryController.GetInstance().ItemsArray[_cell.Index].Amount.ToString();
                break;
            case CellTypeEnum.Player:
                _text.text = InventoryController.GetInstance().PlayerItems[_cell.Index].Amount.ToString();
                break;
            case CellTypeEnum.Chest:
                _text.text = InventoryController.GetInstance().ChestItems[_cell.Index].Amount.ToString();
                break;
        }
    }

    public void UnpinItem()
    {
        _cell = null;
        _image.gameObject.SetActive(false);
        _text.gameObject.SetActive(false);
        _slider.gameObject.SetActive(false);
        _waterSlider.gameObject.SetActive(false);
    }
}
