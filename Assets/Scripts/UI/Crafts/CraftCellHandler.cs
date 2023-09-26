using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class CraftCellHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _name;

    private CraftModel _craft;
    private BuildController _controller;

    public void OpenCraftInformation()
    {
        UIController.GetInstance().OpenCraftInformation(_craft, _controller);
    }

    public void DrawCellInformation(CraftModel craft, BuildController controller)
    {
        _craft = craft;
        _controller = controller;
        _name.text = craft._name;
        _itemIcon.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)craft._creftedItem._id); 
    }
}
