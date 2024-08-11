using UnityEngine;

public sealed class BehaviorTreeRunner : MonoBehaviour
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
