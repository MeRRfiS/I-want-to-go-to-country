using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum StateEnum
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public StateEnum State = StateEnum.Running;
    [HideInInspector] public bool Started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 Position;
    [HideInInspector] public Blackboard Blackboard;
    [HideInInspector] public NPCController NPC;
    [TextArea] public string Description;

    public StateEnum Update()
    {
        if (!Started)
        {
            OnStart();
            Started = true;
        }

        State = OnUpdate();

        if(State == StateEnum.Failure ||  State == StateEnum.Success)
        {
            OnStop();
            Started = false;
        }

        return State;
    }

    public void Stop()
    {
        OnStop();
        State = StateEnum.Success;
        Started = false;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract StateEnum OnUpdate();
}
