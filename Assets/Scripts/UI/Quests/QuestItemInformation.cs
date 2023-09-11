using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemInformation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _count;

    public void DrawQuestItemInformation(QuestItem questItem)
    {
        _icon.sprite = Resources.Load<Sprite>(ResourceConstants.ITEMS_ICON + (ItemIdsEnum)questItem._item._id);
        _name.text = ((ItemIdsEnum)questItem._item._id).ToString();
        _count.text = questItem._amount.ToString();
    }
}
