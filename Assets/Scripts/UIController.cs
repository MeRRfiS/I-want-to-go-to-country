using System;
using System.Collections;
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

    private bool _usingProgressBar = false;
    private int _timerMultiple = 1;
    private float _indicatorTimer = 0.0f;
    private float _maxIndicatorTimer = 1.0f;
    [Header("Other")]
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

        var items = InventoryController.GetInstance().ItemsArray;
        for (int i = 0; i < GlobalConstants.MAX_ITEMS_IN_INVENTORY; i++)
        {
            if (items[i] == null) continue;

            Image image = _cells[i].GetChild(0).GetComponent<Image>();
            image.gameObject.SetActive(state);
            image.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + items[i].Id);
            switch (items[i].Type)
            {
                case ItemTypeEnum.Instrument:
                    Slider slider = _cells[i].GetChild(2).GetComponent<Slider>();
                    slider.gameObject.SetActive(state);
                    slider.maxValue = (items[i] as Instrument).Durability;
                    break;
                case ItemTypeEnum.Seed:
                case ItemTypeEnum.Tree:
                case ItemTypeEnum.Harvest:
                    TextMeshProUGUI text = _cells[i].GetChild(1).GetComponent<TextMeshProUGUI>();
                    text.gameObject.SetActive(state);
                    text.text = items[i].Count.ToString();
                    break;
                default:
                    break;
            }
        }
        PlayerController.GetInstance().IsCanMoving = !state;
        PlayerController.GetInstance().IsCanRotation = !state;

        if (state) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.None;
        Cursor.visible = state;
        _inventory.SetActive(state);
    }
}
