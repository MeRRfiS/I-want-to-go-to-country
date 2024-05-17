using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndNode : CompositeNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override StateEnum OnUpdate()
    {
        int successCount = 0;

        foreach (var child in Children) 
        {
            switch (child.Update())
            {
                case StateEnum.Running:
                    break;
                case StateEnum.Failure:
                    break;
                case StateEnum.Success:
                    successCount++;
                    break;
            }
        }

        return successCount == Children.Count ? StateEnum.Success : StateEnum.Running;
    }
}
