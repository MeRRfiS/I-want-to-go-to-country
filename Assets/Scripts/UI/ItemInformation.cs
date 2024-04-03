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
        _icon.sprite = questItem._item.Icon;
        _name.text = ((ItemIdsEnum)questItem._item.Id).ToString();
        _count.text = questItem._amount.ToString();
    }

    public void DrawItemInformation(NeededItem questItem, int amount)
    {
        _icon.sprite = questItem._item.Icon;
        _name.text = ((ItemIdsEnum)questItem._item.Id).ToString();
        _count.text = amount.ToString();
    }
}
