using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsContainer : ScriptableObject
{
    public List<QuestModel> Container { get; set; }

    public virtual void Init() 
    {
        Container = new List<QuestModel>();
    }

    public virtual bool AddQuest(QuestModel newQuest) 
    { 
        Container.Add(newQuest);

        return true;
    }

    public virtual void RemoveQuest(QuestModel removedQuest) 
    {
        foreach (var quest in Container) 
        { 
            if(quest._id == removedQuest._id)
            {
                Container.Remove(quest);
                break;
            }
        }
    }
}
