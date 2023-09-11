using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    [Header("Information")]
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _money;

    [Header("Inventory Menu")]
    [SerializeField] private GameObject _inventory;
    [SerializeField] private List<Transform> _cells;
    [SerializeField] private List<Transform> _playerCells;

    [Header("Shop Menu")]
    [SerializeField] private GameObject _shop;
    [SerializeField] private Transform _buyItemMenu;
    [SerializeField] private Transform _sellItemMenu;

    [Header("Quest Menu")]
    [SerializeField] private GameObject _questMenu;
    [SerializeField] private GameObject _dayQuestObject;
    [SerializeField] private GameObject _playerQuestObject;
    [SerializeField] private Transform _dayQuestList;
    [SerializeField] private Transform _playerQuestList;
    [SerializeField] private TextMeshProUGUI _questAmount;
    [SerializeField] private QuestHandler _questInformation;

    [Header("Quest Menu")]
    [SerializeField] private GameObject _craftMenu;
    [SerializeField] private Transform _craftList;
    [SerializeField] private CraftHandler _craftInformation;

    [Header("Prefabs")]
    [SerializeField] private GameObject _buyShopCell;
    [SerializeField] private GameObject _sellShopCell;
    [SerializeField] private GameObject _quest;
    [SerializeField] private GameObject _craft;

    [Header("Setting")]
    [SerializeField] private Color _deselectedColor;
    [SerializeField] private Color _selectedColor;

    private bool _usingProgressBar = false;
    private int _timerMultiple = 1;
    private float _indicatorTimer = 0.0f;
    private float _maxIndicatorTimer = 1.0f;
    private string _timeTextFormat;
    [Header("Other")]
    [SerializeField] private Image radialIndicatorUI;
    [SerializeField] private PinUpItemInfo _pinUp;
    private UnityEvent eventProgressBar = new UnityEvent();

    public static UIController GetInstance() => instance;
    public bool InventoryActiveSelf() => _inventory.activeSelf;
    public bool ShopActiveSelf() => _shop.activeSelf;
    public bool QuestMenuActiveSelf() => _questMenu.activeSelf;
    public bool CraftMenuActiveSelf() => _craftMenu.activeSelf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _timeTextFormat = _time.text;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        UpdateProgressBar();
        ApplyMovementPinUpTransform();
        UpdateMoneyCountText();
        UpdateQuestAmountText();
        UpdateTimeText();
    }

    private void UpdateMoneyCountText()
    {
        _money.text = PlayerController.GetInstance().Money.ToString();
    }

    private void UpdateQuestAmountText()
    { 
        _questAmount.text = QuestSystemController.GetInstance().PlayerContainer.Container.Count.ToString();
    }

    private void UpdateTimeText()
    {
        float timeValue = WorldController.GetInstance().TimeOfDay;
        int hours = (int)Math.Truncate(timeValue);
        int minutes = (int)Math.Floor((timeValue - hours) * UIConstants.MINUTES_MULTIPLIER);
        string timeText = String.Format(_timeTextFormat, hours, minutes < 10 ? $"0{minutes}" : minutes);
        _time.text = timeText;
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
        _pinUp.transform.position = Input.mousePosition;
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
                default:
                    break;
            }
        }
    }

    private void RedrawQuestMenu(List<QuestModel> questList, Transform list, QuestTypeEnum type)
    {
        for (int i = list.childCount - 1; i >= 0; i--)
        {
            Destroy(list.GetChild(i).gameObject);
        }
        foreach (var quest in questList)
        {
            QuestCellHandler questCell = Instantiate(_quest, list).GetComponent<QuestCellHandler>();
            questCell.DrawCellInformation(quest);
            questCell.QuestType = type;
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

    public void SwitchActiveQuestMenu()
    {
        bool state = !_questMenu.activeSelf;

        InventoryController.GetInstance().IsCanChangeActiveItem = !state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        CloseQuestInformation();
        _dayQuestObject.SetActive(state);
        _playerQuestObject.SetActive(!state);
        SwitchActiveMouse(state);
        _questMenu.SetActive(state);
    }

    public void SwitchActiveCraftMenu()
    {
        bool state = !_craftMenu.activeSelf;

        InventoryController.GetInstance().IsCanChangeActiveItem = !state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        CloseCraftInformation();
        SwitchActiveMouse(state);
        _craftMenu.SetActive(state);
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

    public void RedrawDayQuestMenu(List<QuestModel> questList)
    {
        RedrawQuestMenu(questList, _dayQuestList, QuestTypeEnum.DayQuest);
    }

    public void RedrawPlayerQuestMenu(List<QuestModel> questList)
    {
        RedrawQuestMenu(questList, _playerQuestList, QuestTypeEnum.PlayerQuest);
    }

    public void RedrawCraftMenu(List<CraftModel> craftList, CraftController controller)
    {
        for (int i = _craftList.childCount - 1; i >= 0; i--)
        {
            Destroy(_craftList.GetChild(i).gameObject);
        }
        foreach (var craft in craftList)
        {
            CraftCellHandler craftCell = Instantiate(_craft, _craftList).GetComponent<CraftCellHandler>();
            craftCell.DrawCellInformation(craft, controller);
        }
    }

    public void OpenQuestInformation(QuestModel quest, QuestTypeEnum typeQuest)
    {
        _questInformation.DrawQuestInformation(quest, typeQuest);
        _questInformation.gameObject.SetActive(true);
    }

    public void CloseQuestInformation()
    {
        _questInformation.gameObject.SetActive(false);
    }

    public void OpenCraftInformation(CraftModel craft, CraftController controller)
    {
        _craftInformation.DrawCraftInformation(craft, controller);
        _craftInformation.gameObject.SetActive(true);
    }

    public void CloseCraftInformation()
    {
        _craftInformation.gameObject.SetActive(false);
    }

    public void OpenDayQuestMenu()
    {
        CloseQuestInformation();
        _playerQuestObject.SetActive(false);
        RedrawDayQuestMenu(QuestSystemController.GetInstance().DayContainer.Container);
        _dayQuestObject.SetActive(true);
    }

    public void OpenPlayerQuestMenu()
    {
        CloseQuestInformation();
        _dayQuestObject.SetActive(false);
        RedrawPlayerQuestMenu(QuestSystemController.GetInstance().PlayerContainer.Container);
        _playerQuestObject.SetActive(true);
    }

    public void PinUpItemToMouse(int index = -1, CellTypeEnum type = CellTypeEnum.None)
    {
        if (_pinUp.IsItemPin()) return;

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

        _pinUp.PinUpItem(cell);
    }

    public void UpdatePinItemInfo()
    {
        _pinUp.UpdatePinUpItemInformation();
    }

    public void UnpinItemFromMouse()
    {
        _pinUp.UnpinItem();
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
