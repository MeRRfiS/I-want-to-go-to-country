using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _current;

    protected override void OnStart()
    {
        _current = 0;
    }

    protected override void OnStop()
    {
        foreach (var child in Children)
        {
            child.Stop();
        }
    }

    protected override StateEnum OnUpdate()
    {
        var child = Children[_current];
        switch (child.Update())
        {
            case StateEnum.Running:
                return StateEnum.Running;
            case StateEnum.Failure:
                return StateEnum.Failure;
            case StateEnum.Success:
                _current++;
                break;
        }

        return _current == Children.Count ? StateEnum.Success : StateEnum.Running;
    }
}
