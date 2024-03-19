using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestSystemController : MonoBehaviour
{
    private static QuestSystemController _instance;

    [Header("Quest Collection")]
    [SerializeField] private QuestsCollection _collection;

    [Header("Quest's Containers")]
    [SerializeField] private QuestsContainer _playerContainer;
    [SerializeField] private QuestsContainer _dayContainer;

    public static QuestSystemController GetInstance() => _instance;

    public QuestsContainer DayContainer
    {
        get => _dayContainer;
    }

    public QuestsContainer PlayerContainer 
    {
        get => _playerContainer;
    }

    private bool IsHasQuestItem(List<Item> items, QuestModel quest)
    {
        foreach (var questItem in quest._neededItems)
        {
            List<Item> neededItems = items.Where(item => 
            {
                if(item == null) return false;
                if(item.Id != questItem._item.Id) return false;

                return true;
            }).ToList();
            if (neededItems.Count == 0) return false;

            int itemsAmount = neededItems.Select(item => item.Amount).Sum();
            if (itemsAmount < questItem._amount) return false;
        }

        return true;
    }

    private void Awake()
    {
        _playerContainer.Init();
        _dayContainer.Init();
        _instance = this;
    }

    private void Start()
    {
        InitializeDayQuests();
    }

    public void InitializeDayQuests()
    {
        _dayContainer.Container.Clear();

        for (int i = 0; i < MechConstants.MAX_DAY_QUEST; i++)
        {
            bool questIsAdded = false;
            while (!questIsAdded)
            {
                int indexQuest = Random.Range(0, _collection.Quests.Count);
                questIsAdded = _dayContainer.AddQuest(_collection.Quests[indexQuest]);
            }
        }
    }

    public void LoadDayQuestListToUI()
    {
        UIController.GetInstance().RedrawDayQuestMenu(_dayContainer.Container);
    }

    public void LoadPlayerQuestListToUI()
    {
        UIController.GetInstance().RedrawPlayerQuestMenu(_playerContainer.Container);
    }

    public bool IsQuestCompleted(QuestModel quest)
    {
        Item[] inventoryItems = InventoryController.GetInstance().ItemsArray;
        Item[] playerItems = InventoryController.GetInstance().PlayerItems;
        List<Item> allItems = new List<Item>();
        foreach (var item in inventoryItems)
        {
            allItems.Add(item);
        }
        foreach (var item in playerItems)
        {
            allItems.Add(item);
        }

        bool isHasItem = IsHasQuestItem(allItems, quest);

        return isHasItem;
    }
}
