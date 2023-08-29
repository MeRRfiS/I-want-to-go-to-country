using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quests Container", menuName = "Quest System/Day Quest Container")]
public class DayQuests : QuestsContainer
{
    public override bool AddQuest(QuestModel newQuest)
    {
        int foundIndex = Container.Where(q => q._id == newQuest._id).Count();

        if(foundIndex != 0) return false;
        Container.Add(newQuest);

        return true;
    }
}
