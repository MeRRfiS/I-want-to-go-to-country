using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeNode : DecoratorNode
{
    [Header("Work to (time)")]
    [SerializeField] private int _hour = 0;
    [SerializeField] private int _minute = 0;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        Child.Stop();
    }

    protected override StateEnum OnUpdate()
    {
        StateEnum state;
        float hourOfDay = WorldController.GetInstance().HourOfDay;
        float minuteOfDay = WorldController.GetInstance().MinuteOfDay;

        if (hourOfDay != _hour || minuteOfDay != _minute)
        {
            Child.Update();
            state = StateEnum.Running;
        }
        else
        {
            state = StateEnum.Success;
        }

        return state;
    }
}
