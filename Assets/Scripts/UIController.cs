using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;

    [Header("Menus")]
    [SerializeField] private GameObject _inventory;
    [SerializeField] private List<Transform> _cells;
    [SerializeField] private List<Transform> _playerCells;

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

    private void RedrawInventory(Item[] items, List<Transform> cells)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Image image = cells[i].GetChild(0).GetComponent<Image>();
            TextMeshProUGUI text = cells[i].GetChild(1).GetComponent<TextMeshProUGUI>();
            Slider slider = cells[i].GetChild(2).GetComponent<Slider>();
            if (items[i] == null)
            {
                image.sprite = null;
                image.gameObject.SetActive(false);
                text.text = String.Empty;
                text.gameObject.SetActive(false);
                slider.gameObject.SetActive(false);
                continue;
            }

            image.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + items[i].Id);
            image.gameObject.SetActive(true);
            switch (items[i].Type)
            {
                case ItemTypeEnum.Instrument:
                    slider.maxValue = (items[i] as Instrument).Durability;
                    slider.gameObject.SetActive(true);
                    text.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Seed:
                case ItemTypeEnum.Tree:
                case ItemTypeEnum.Harvest:
                    text.text = items[i].Count.ToString();
                    text.gameObject.SetActive(true);
                    slider.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
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
        PlayerController.GetInstance().IsCanMoving = !state;
        PlayerController.GetInstance().IsCanRotation = !state;
        PlayerController.GetInstance().IsCanUsingItem = !state;

        _pinUp.gameObject.SetActive(state);
        if (state) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.None;
        Cursor.visible = state;
        _inventory.SetActive(state);
    }

    public void RedrawInventories()
    {
        //Redrawing main Inventory
        RedrawInventory(InventoryController.GetInstance().ItemsArray, _cells);

        //Redrawing player Inventory
        RedrawInventory(InventoryController.GetInstance().PlayerItems, _playerCells);
    }

    public void PinUpItemToMouse(int index = -1, CellTypeEnum type = CellTypeEnum.None)
    {
        if (_pinUp.childCount == 0)
        {
            Image image = null;
            TextMeshProUGUI text = null;
            Slider slider = null;
            switch (type)
            {
                case CellTypeEnum.Inventory:
                    image = _cells[index].GetChild(0).GetComponent<Image>();
                    text = _cells[index].GetChild(1).GetComponent<TextMeshProUGUI>();
                    slider = _cells[index].GetChild(2).GetComponent<Slider>();
                    break;
                case CellTypeEnum.Player:
                    image = _playerCells[index].GetChild(0).GetComponent<Image>();
                    text = _playerCells[index].GetChild(1).GetComponent<TextMeshProUGUI>();
                    slider = _playerCells[index].GetChild(2).GetComponent<Slider>();
                    break;
            }
            Instantiate(image, _pinUp);
            Instantiate(text, _pinUp);
            Instantiate(slider, _pinUp);
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            slider.gameObject.SetActive(false); 
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
