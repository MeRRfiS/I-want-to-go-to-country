using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node Child;

    public override Node Clone()
    {
        DecoratorNode node = Instantiate(this);
        node.Child = Child.Clone();

        return node;
    }
}
