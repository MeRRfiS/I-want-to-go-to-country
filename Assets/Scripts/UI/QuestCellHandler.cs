using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class QuestCellHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _robotIcon;
    [SerializeField] private TextMeshProUGUI _name;

    private QuestTypeEnum _type;
    private QuestModel _quest;

    public QuestTypeEnum QuestType
    {
        set => _type = value;
    }

    public void OpenQuestInformation()
    {
        UIController.GetInstance().OpenQuestInformation(_quest, _type);
    }

    public void DrawCellInformation(QuestModel quest)
    {
        _quest = quest;
        _name.text = _quest._name;
    }
}
