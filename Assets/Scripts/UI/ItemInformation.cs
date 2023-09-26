using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInformation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _count;

    public void DrawItemInformation(NeededItem questItem)
    {
        _icon.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)questItem._item._id);
        _name.text = ((ItemIdsEnum)questItem._item._id).ToString();
        _count.text = questItem._amount.ToString();
    }

    public void DrawItemInformation(NeededItem questItem, int amount)
    {
        _icon.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)questItem._item._id);
        _name.text = ((ItemIdsEnum)questItem._item._id).ToString();
        _count.text = amount.ToString();
    }
}
