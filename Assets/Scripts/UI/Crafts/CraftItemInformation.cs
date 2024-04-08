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

    public void DrawItemInformation(NeededItem item)
    {
        _icon.sprite = item._item.Icon;
        _name.text = ((ItemIdsEnum)item._item.Id).ToString();
        _count.text = item._amount.ToString();
    }
}
