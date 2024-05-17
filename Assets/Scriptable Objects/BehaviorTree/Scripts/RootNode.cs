using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    [HideInInspector] public Node Child;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override StateEnum OnUpdate()
    {
        return Child.Update();
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.Child = Child.Clone();

        return node;
    }
}
