using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quests Collection", menuName = "Quest System/Quests")]
public class QuestsCollection : ScriptableObject
{
    [SerializeField] private List<QuestModel> _quests = new List<QuestModel>();

    public List<QuestModel> Quests
    {
        get => _quests;
    }
}

[System.Serializable]
public class QuestModel
{
    public int _id;
    public string _name;
    public List<QuestItem> _questItems = new List<QuestItem>();
    public int _reward;
}

[System.Serializable]
public class QuestItem
{
    public Item _item;
    public int _amount;
}
