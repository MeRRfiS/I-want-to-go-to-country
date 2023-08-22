using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    [Header("Menus")]
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _shop;
    [SerializeField] private Transform _buyItemMenu;
    [SerializeField] private Transform _sellItemMenu;
    [SerializeField] private List<Transform> _cells;
    [SerializeField] private List<Transform> _playerCells;
    [SerializeField] private TextMeshProUGUI _money;

    [Header("Prefabs")]
    [SerializeField] private GameObject _buyShopCell;
    [SerializeField] private GameObject _sellShopCell;

    [Header("Setting")]
    [SerializeField] private Color _deselectedColor;
    [SerializeField] private Color _selectedColor;

    private bool _usingProgressBar = false;
    private int _timerMultiple = 1;
    private float _indicatorTimer = 0.0f;
    private float _maxIndicatorTimer = 1.0f;
    [Header("Other")]
    [SerializeField] private Transform _pinUp;
    [SerializeField] private Image radialIndicatorUI;
    private UnityEvent eventProgressBar = new UnityEvent();

    public static UIController GetInstance() => instance;
    public bool MenuActiveSelf() => _inventory.activeSelf || _shop.activeSelf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        UpdateProgressBar();
        ApplyMovementPinUpTransform();
        UpdateMoneyCountText();
    }

    private void UpdateMoneyCountText()
    {
        _money.text = PlayerController.GetInstance().Money.ToString();
    }

    private void UpdateProgressBar()
    {
        if (PlayerInputSystem.holdingLMB && _usingProgressBar)
        {
            _indicatorTimer += Time.deltaTime / _timerMultiple;
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = _indicatorTimer;

            if (_indicatorTimer >= _maxIndicatorTimer)
            {
                _indicatorTimer = 0.0f;
                radialIndicatorUI.fillAmount = 0.0f;
                eventProgressBar.Invoke();
            }
        }
        else
        {
            _indicatorTimer = 0.0f;
            radialIndicatorUI.fillAmount = 0.0f;
            radialIndicatorUI.enabled = false;
            _usingProgressBar = false;
            eventProgressBar.RemoveAllListeners();
        }
    }

    private void ApplyMovementPinUpTransform()
    {
        _pinUp.position = Input.mousePosition;
    }

    //TODO: Make method more readable
    private void RedrawInventory(Item[] items, List<Transform> cells)
    {
        for (int i = 0; i < items.Length; i++)
        {
            InventoryCellHandler cell = cells[i].GetComponent<InventoryCellHandler>();
            Image image = cell.ItemIcon;
            TextMeshProUGUI text = cell.TextCount;
            Slider slider = cell.SliderDurability;
            if (items[i] == null)
            {
                image.sprite = null;
                image.gameObject.SetActive(false);
                text.text = String.Empty;
                text.gameObject.SetActive(false);
                slider.gameObject.SetActive(false);
                continue;
            }

            image.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)items[i]._id);
            image.gameObject.SetActive(true);
            switch (items[i]._type)
            {
                case ItemTypeEnum.Instrument:
                    Instrument instrument = items[i] as Instrument;
                    if (instrument._instrumentType == InstrumentTypeEnum.Funnel)
                    {
                        Funnel funnel = items[i] as Funnel;
                        cell.WaterValueSlider.maxValue = funnel._maxUsings;
                        cell.WaterValueSlider.value = funnel.Usings;
                        cell.WaterValueSlider.gameObject.SetActive(true);
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
                default:
                    break;
            }
        }
    }

    private void SwitchActiveMouse(bool state)
    {
        if (state) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.None;
        Cursor.visible = state;
    }

    public void ProgressBar(int timerMultiple, UnityAction action)
    {
        if (_usingProgressBar) return;

        eventProgressBar.AddListener(action);
        _timerMultiple = timerMultiple;
        _usingProgressBar = true;
    }

    public void StopProgressBar()
    {
        eventProgressBar.RemoveAllListeners();
        _usingProgressBar = false;
    }

    public void SwitchActiveInventoryMenu()
    {
        bool state = !_inventory.activeSelf;

        RedrawInventory(InventoryController.GetInstance().ItemsArray, _cells);
        InventoryController.GetInstance().IsCanChangeActiveItem = !state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        _pinUp.gameObject.SetActive(state);
        SwitchActiveMouse(state);
        _inventory.SetActive(state);
    }

    public void SwitchActiveShopMenu()
    {
        bool state = !_shop.activeSelf;

        InventoryController.GetInstance().IsCanChangeActiveItem = !state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        SwitchActiveMouse(state);
        if (!state)
        {
            for (int i = _buyItemMenu.childCount - 1; i >= 0; i--)
            {
                Destroy(_buyItemMenu.GetChild(i).gameObject);
            }
        }
        _shop.SetActive(state);
    }

    public void RedrawInventories()
    {
        //Redrawing main Inventory
        RedrawInventory(InventoryController.GetInstance().ItemsArray, _cells);

        //Redrawing player Inventory
        RedrawInventory(InventoryController.GetInstance().PlayerItems, _playerCells);
    }

    public void RedrawShop(List<GoodsModel> goodsForDay, ShopController shop)
    {
        for (int i = _buyItemMenu.childCount - 1; i >= 0; i--)
        {
            Destroy(_buyItemMenu.GetChild(i).gameObject);
        }
        for (int i = 0; i < goodsForDay.Count; i++)
        {
            if (goodsForDay[i] == null) continue;

            ShopCellHandler cell = Instantiate(_buyShopCell, _buyItemMenu).GetComponent<ShopCellHandler>();
            cell.DrawCellInformation(goodsForDay[i], shop);
            cell.Index = i;
        }
    }

    public void RedrawShop(Dictionary<int, GoodsModel> sellingGoods, ShopController shop)
    {
        for (int i = _sellItemMenu.childCount - 1; i >= 0; i--)
        {
            Destroy(_sellItemMenu.GetChild(i).gameObject);
        }
        foreach (var goods in sellingGoods)
        {
            ShopCellHandler cell = Instantiate(_sellShopCell, _sellItemMenu).GetComponent<ShopCellHandler>();
            cell.DrawCellInformation(goods.Value, shop);
            cell.Index = goods.Key;
        }
    }

    public void PinUpItemToMouse(int index = -1, CellTypeEnum type = CellTypeEnum.None)
    {
        if (_pinUp.childCount == 0)
        {
            InventoryCellHandler cell = null;
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    if (InventoryController.GetInstance().ItemsArray[index] == null) return;
                    cell = _cells[index].GetComponent<InventoryCellHandler>();
                    break;
                case CellTypeEnum.Player:
                    if (InventoryController.GetInstance().PlayerItems[index] == null) return;
                    cell = _playerCells[index].GetComponent<InventoryCellHandler>();
                    break;
            }
            Image image = cell.ItemIcon;
            TextMeshProUGUI text = cell.TextCount;
            Slider slider = cell.SliderDurability;
            Slider waterSlider = cell.WaterValueSlider;
            Instantiate(image, _pinUp);
            Instantiate(text, _pinUp);
            Instantiate(slider, _pinUp);
            Instantiate(waterSlider, _pinUp);
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            slider.gameObject.SetActive(false);
            waterSlider.gameObject.SetActive(false);
        }
        else
        {
            for (int i = _pinUp.childCount - 1; i >= 0; i--)
            {
                Destroy(_pinUp.GetChild(i).gameObject);
            }
        }
    }

    public void SelectingPlayerCell(int index)
    {
        for (int i = 0; i < _playerCells.Count; i++)
        {
            if (i == index) 
            {
                _playerCells[index].GetComponent<Image>().color = _selectedColor;
            }
            else
            {
                _playerCells[i].GetComponent<Image>().color = _deselectedColor;
            }
        }
    }
}
