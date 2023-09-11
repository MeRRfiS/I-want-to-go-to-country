using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _itemList;
    [SerializeField] private TextMeshProUGUI _reward;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Button _button;

    [Header("Prefab")]
    [SerializeField] private GameObject _itemInfo;

    private QuestModel _quest;

    private void TakeQuest()
    {
        if (QuestSystemController.GetInstance().PlayerContainer.Container.Count ==
            MechConstants.MAX_PLAYER_AMOUNT_QUEST) return;

        QuestSystemController.GetInstance().DayContainer.RemoveQuest(_quest);
        QuestSystemController.GetInstance().PlayerContainer.AddQuest(_quest);
        QuestSystemController.GetInstance().LoadDayQuestListToUI();
        UIController.GetInstance().CloseQuestInformation();
    }

    private void PassQuest()
    {
        bool isQuestCompleted = QuestSystemController.GetInstance().IsQuestCompleted(_quest);
        if (isQuestCompleted)
        {
            foreach (var questItem in _quest._questItems)
            {
                InventoryController.GetInstance().RemoveItem(questItem._item, questItem._amount);
            }
            PlayerController.GetInstance().Money += _quest._reward;
            QuestSystemController.GetInstance().PlayerContainer.RemoveQuest(_quest);
            QuestSystemController.GetInstance().LoadPlayerQuestListToUI();
            UIController.GetInstance().RedrawInventories();
            UIController.GetInstance().CloseQuestInformation();
        }
    }

    public void DrawQuestInformation(QuestModel quest, QuestTypeEnum type)
    {
        for (int i = 0; i < _itemList.childCount; i++)
        {
            Destroy(_itemList.GetChild(i).gameObject);
        }

        _quest = quest;
        foreach (var questItem in quest._questItems) 
        { 
            QuestItemInformation QI_Information = Instantiate(_itemInfo, _itemList).GetComponent<QuestItemInformation>();
            QI_Information.DrawQuestItemInformation(questItem);
        }
        _reward.text = quest._reward.ToString();

        _button.onClick.RemoveAllListeners();
        switch (type)
        {
            case QuestTypeEnum.DayQuest:
                _buttonText.text = "Take";
                _button.onClick.AddListener(TakeQuest);
                break;
            case QuestTypeEnum.PlayerQuest:
                _buttonText.text = "Pass";
                _button.onClick.AddListener(PassQuest);
                break;
        }
    }
}
