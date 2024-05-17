using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    public float Duration = 1;
    private float _startTime;

    protected override void OnStart()
    {
        _startTime = Time.time;
    }

    protected override void OnStop()
    {
        
    }

    protected override StateEnum OnUpdate()
    {
        if(Time.time - _startTime > Duration)
        {
            return StateEnum.Success;
        }

        return StateEnum.Running;
    }
}
