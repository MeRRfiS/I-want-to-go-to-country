using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftItemInformation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _count;

    public void DrawCraftItemInformation(CraftedItem item)
    {
        _icon.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)item._item._id);
        _name.text = ((ItemIdsEnum)item._item._id).ToString();
        _count.text = item._amount.ToString();
    }
}
