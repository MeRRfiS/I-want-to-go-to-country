using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public BehaviorTree Tree;

    private void Start()
    {
        Tree = Tree.Clone();
        Tree.Bind(GetComponent<NPCController>());
    }

    private void Update()
    {
        Tree.Update();
    }
}
