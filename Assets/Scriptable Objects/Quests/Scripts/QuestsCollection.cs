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
public class QuestModel: InformationModel
{
    public Sprite _robotIcon;
    public enum QuestType
    {
        Easy = 1,
        Normal,
        Hard
    }

    public int _id;
    public int _reward;
    public QuestType _type;
}
