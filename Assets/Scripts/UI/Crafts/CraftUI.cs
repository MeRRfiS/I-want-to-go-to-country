using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _craftMenu;
    [SerializeField] private GameObject _upgradeMenu;

    [Header("Craft's Menu Components")]
    [SerializeField] private GameObject _craftInfo;

    [Header("Update's Menu Components")]
    [SerializeField] private GameObject _updateInfo;
    [SerializeField] private Transform _itemList;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private List<Image> _levelButtonImages;
    [SerializeField] private List<Button> _levelButtons;

    [Header("Settings")]
    [SerializeField] private Color _activeLevel;
    [SerializeField] private Color _nextActiveLevel;
    [SerializeField] private Color _nonActiveLevel;

    [Header("Prefab")]
    [SerializeField] private GameObject _itemInfo;

    private int _openLevel;
    private BuildController _buildController;

    public BuildController BuildController
    {
        set => _buildController = value;
    }
    private bool BlockerInputSystem() => true;

    private void OnEnable()
    {
        PlayerInputSystem.BlockInputSystem += BlockerInputSystem;
        UIInputSystem.BlockInputSystem += BlockerInputSystem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.BlockInputSystem -= BlockerInputSystem;
        UIInputSystem.BlockInputSystem -= BlockerInputSystem;
    }

    private void RedrawLevelButton()
    {
        bool isPastLevelActive = false;
        for (int i = 0; i < MechConstants.MAX_LEVEL_OF_BUILDING; i++)
        {
            if (i + 1 <= _buildController.Level)
            {
                isPastLevelActive = true;
                _levelButtonImages[i].color = _activeLevel;
                _levelButtons[i].interactable = false;
            }
            else
            {
                if (isPastLevelActive)
                {
                    _levelButtonImages[i].color = _nextActiveLevel;
                    _levelButtons[i].interactable = true;
                    isPastLevelActive = false;
                }
                else
                {
                    _levelButtonImages[i].color = _nonActiveLevel;
                    _levelButtons[i].interactable = false;
                }
            }
        }
    }

    public void OpenCraftMenu()
    {
        _upgradeMenu.SetActive(false);

        _updateInfo.SetActive(false);
        _openLevel = 0;

        _craftMenu.SetActive(true);
    }

    public void OpenUpgradeMenu()
    {
        _craftMenu.SetActive(false);

        _craftInfo.SetActive(false);
        RedrawLevelButton();

        _upgradeMenu.SetActive(true);
    }

    public void OpenLevelInformation(int levelNumber)
    {
        _openLevel = levelNumber;
        for (int i = 0; i < _itemList.childCount; i++)
        {
            Destroy(_itemList.GetChild(i).gameObject);
        }

        UpgradeModel upgradeModel = _buildController.UpgradeCollection._upgrades[_openLevel - 1];
        foreach (var material in upgradeModel._neededItems)
        {
            ItemInformation craftInformation = Instantiate(_itemInfo, _itemList).GetComponent<ItemInformation>();
            craftInformation.DrawItemInformation(material);
        }
        _price.text = upgradeModel._price.ToString();
        _updateInfo.SetActive(true);
    }

    public void CloseLevelInformation()
    {
        _updateInfo.SetActive(false);
        _openLevel = 0;
    }

    public void UpgradeButton()
    {
        bool isUpgraded = _buildController.UpgradeBuilding(_openLevel);
        if (isUpgraded)
        {
            RedrawLevelButton();
            _updateInfo.SetActive(false);
            _openLevel = 0;
        }
    }
}
