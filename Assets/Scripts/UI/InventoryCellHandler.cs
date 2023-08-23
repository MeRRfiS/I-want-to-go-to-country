using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class InventoryCellHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Setting")]
    [SerializeField] private int _index;
    [SerializeField] private CellTypeEnum _typeCell;

    [Header("Components")]
    [SerializeField] private Image _image;
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _waterSlider;
    [SerializeField] private TextMeshProUGUI _text;

    public Image ItemIcon
    {
        get => _image;
    }

    public Slider SliderDurability
    {
        get => _slider;
    }

    public Slider WaterValueSlider
    {
        get => _waterSlider;
    }

    public TextMeshProUGUI TextCount
    {
        get => _text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UIController.GetInstance().MenuActiveSelf()) return;

        UIController.GetInstance().PinUpItemToMouse(_index, _typeCell);
        InventoryController.GetInstance().SelectItem(_index, _typeCell);
    }

    private void Update()
    {
        UpdateCellInformation();
    }

    private void UpdateCellInformation()
    {
        if (_typeCell != CellTypeEnum.Player) return;

        Item item = InventoryController.GetInstance().PlayerItems[_index];
        if (item == null) return;

        switch (item._type)
        {
            case ItemTypeEnum.None:
                break;
            case ItemTypeEnum.Instrument:
                Instrument instrument = item as Instrument;
                if (instrument._instrumentType == InstrumentTypeEnum.Funnel)
                {
                    Funnel funnel = item as Funnel;
                    _waterSlider.value = funnel.Usings;
                }
                _slider.value = instrument.Durability;
                break;
            case ItemTypeEnum.Seed:
            case ItemTypeEnum.Tree:
            case ItemTypeEnum.Harvest:
                _text.text = item.Amount.ToString();
                break;
            case ItemTypeEnum.Fertilizers:
                Fertilizers fertilizers = item as Fertilizers;
                _slider.value = fertilizers.Usings;
                break;
            default:
                break;
        }
    }
}