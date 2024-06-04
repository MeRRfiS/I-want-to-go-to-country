using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    [Header("Information")]
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _fps;
    [SerializeField] private TextMeshProUGUI _money;
    [SerializeField] private GameObject _buildInfo;

    [Header("Game Menu")]
    [SerializeField] private GameMenuUIManager _gameMenu;

    [Header("Inventory Menu")]
    [SerializeField] private InventoryUI _mainInventory;
    [SerializeField] private InventoryUI _playerInventory;

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

    [Header("Craft Menu")]
    [SerializeField] private CraftUI _craftMenu;
    [SerializeField] private Transform _craftList;
    [SerializeField] private CraftHandler _craftInformation;

    [Header("Chest Menu")]
    [SerializeField] private GameObject _chestMenu;
    [SerializeField] private InventoryUI _chestInventory;
    [SerializeField] private InventoryUI _chestMainInventory;

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
    private int _frameCount = 0;
    private float _indicatorTimer = 0.0f;
    private float _maxIndicatorTimer = 1.0f;
    private float _nextFPSUpdate;
    private string _timeTextFormat;
    private string _fpsTextFormat;
    private string _moneyTextFormat;
    [Header("Other")]
    [SerializeField] private Image radialIndicatorUI;
    [SerializeField] private PinUpItemInfo _pinUp;
    private UnityEvent eventProgressBar = new UnityEvent();

    public static UIController GetInstance() => instance;
    public void ChangeBuildInfoActive(bool active) => _buildInfo.SetActive(active);
    public bool GameMenuActiveSelf() => _gameMenu.gameObject.activeSelf;
    public bool InventoryActiveSelf() => _mainInventory.gameObject.activeSelf;
    public bool ShopActiveSelf() => _shop.activeSelf;
    public bool QuestMenuActiveSelf() => _questMenu.activeSelf;
    public bool CraftMenuActiveSelf() => _craftMenu.gameObject.activeSelf;
    public bool ChestMenuActiveSelf() => _chestMenu.activeSelf;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _nextFPSUpdate = Time.time;
        _timeTextFormat = _time.text;
        _fpsTextFormat = _fps.text;
        _moneyTextFormat = _money.text;
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
        UpdateFPS();
    }

    private void UpdateMoneyCountText()
    {
        //_money.text = $"{ShopCellHandler.COIN_SPRITE} {PlayerController.GetInstance().Money}";
        _money.text = string.Format(_moneyTextFormat, PlayerController.GetInstance().Money);
    }

    private void UpdateQuestAmountText()
    { 
        _questAmount.text = QuestSystemController.GetInstance().PlayerContainer.Container.Count.ToString();
    }

    private void UpdateTimeText()
    {
        int hours = WorldController.GetInstance().HourOfDay;
        int minutes = WorldController.GetInstance().MinuteOfDay;
        string timeText = String.Format(_timeTextFormat, hours, minutes < 10 ? $"0{minutes}" : minutes);
        _time.text = timeText;
    }

    private void UpdateFPS()
    {
        _frameCount++;

        if(Time.time > _nextFPSUpdate)
        {
            int fps = 0;
            _nextFPSUpdate += 1f / 4f;
            fps = _frameCount * 4;
            _frameCount = 0;

            string fpsText = String.Format(_fpsTextFormat, fps);
            _fps.text = fpsText;
        }
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
        if (!state) Cursor.lockState = CursorLockMode.Locked;
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

    public void SwitchActiveGameMenu()
    {
        bool state = !_gameMenu.gameObject.activeSelf;

        InventoryController.GetInstance().IsOpenUI = state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        SwitchActiveMouse(state);
        _gameMenu.gameObject.SetActive(state);
    }

    public void SwitchActiveInventoryMenu()
    {
        bool state = !_mainInventory.gameObject.activeSelf;

        _mainInventory.RedrawInventory();
        InventoryController.GetInstance().IsOpenUI = state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        _pinUp.gameObject.SetActive(state);
        SwitchActiveMouse(state);
        _mainInventory.gameObject.SetActive(state);
    }

    public void SwitchActiveShopMenu()
    {
        bool state = !_shop.activeSelf;

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

        PlayerController.GetInstance().SwitchActiveController(!state);

        CloseQuestInformation();
        _dayQuestObject.SetActive(state);
        _playerQuestObject.SetActive(!state);
        SwitchActiveMouse(state);
        _questMenu.SetActive(state);
    }

    public void SwitchActiveCraftMenu()
    {
        bool state = !_craftMenu.gameObject.activeSelf;

        InventoryController.GetInstance().IsOpenUI = state;
        PlayerController.GetInstance().SwitchActiveController(!state);

        CloseCraftInformation();
        SwitchActiveMouse(state);
        _craftMenu.OpenCraftMenu();
        _craftMenu.gameObject.SetActive(state);
    }

    public void SwitchActiveChestMenu()
    {
        bool state = !_chestMenu.activeSelf;

        InventoryController.GetInstance().IsOpenUI = state;
        PlayerController.GetInstance().SwitchActiveController(!state);
        _chestMainInventory.RedrawInventory();

        _pinUp.gameObject.SetActive(state);
        SwitchActiveMouse(state);
        _chestMenu.SetActive(state);
    }

    public void RedrawInventories()
    {
        if (InventoryActiveSelf())
        {
            _mainInventory.RedrawInventory();
        }
        else if (ChestMenuActiveSelf())
        {
            _chestInventory.RedrawInventory();
            _chestMainInventory.RedrawInventory();
        }

        _playerInventory.RedrawInventory();
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

    public void RedrawCraftMenu(List<CraftModel> craftList, BuildController controller)
    {
        _craftMenu.BuildController = controller;
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

    public void OpenCraftInformation(CraftModel craft, BuildController controller)
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
                if (InventoryActiveSelf())
                {
                    cell = _mainInventory.Cells[index].GetComponent<InventoryCellHandler>();
                }
                else if (ChestMenuActiveSelf())
                {
                    cell = _chestMainInventory.Cells[index].GetComponent<InventoryCellHandler>();
                }
                break;
            case CellTypeEnum.Player:
                if (InventoryController.GetInstance().PlayerItems[index] == null) return;
                cell = _playerInventory.Cells[index].GetComponent<InventoryCellHandler>();
                break;
            case CellTypeEnum.Chest:
                if (InventoryController.GetInstance().ChestItems[index] == null) return;
                cell = _chestInventory.Cells[index].GetComponent<InventoryCellHandler>();
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
        for (int i = 0; i < MechConstants.MAX_ITEMS_IN_PLAYER; i++)
        {
            if (i == index) 
            {
                _playerInventory.Cells[index].GetComponent<Image>().color = _selectedColor;
            }
            else
            {
                _playerInventory.Cells[i].GetComponent<Image>().color = _deselectedColor;
            }
        }
    }
}
