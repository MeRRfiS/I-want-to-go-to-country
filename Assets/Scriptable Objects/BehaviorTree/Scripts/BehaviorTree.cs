using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
    public Node RootNode;
    public Node.StateEnum TreeState = Node.StateEnum.Running;
    public List<Node> Nodes = new List<Node>();
    public Blackboard Blackboard = new Blackboard();

    public Node.StateEnum Update()
    {
        if(RootNode.State == Node.StateEnum.Running)
        {
            TreeState = RootNode.Update();
        }

        return TreeState;
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behavior Tree (CreateNode)");
        Nodes.Add(node);

        if(!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");

        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behavior Tree (DeleteNode)");
        Nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behavior Tree (AddChild)");
            decorator.Child = child;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behavior Tree (AddChild)");
            root.Child = child;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behavior Tree (AddChild)");
            composite.Children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behavior Tree (RemoveChild)");
            decorator.Child = null;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behavior Tree (RemoveChild)");
            root.Child = null;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behavior Tree (RemoveChild)");
            composite.Children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

#endif

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.Child != null)
        {
            children.Add(decorator.Child);
        }

        RootNode root = parent as RootNode;
        if (root && root.Child != null)
        {
            children.Add(root.Child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.Children;
        }

        return children;
    }


    public void Traverse(Node node, Action<Node> visiter)
    {
        if (node)
        {
            visiter?.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviorTree Clone()
    {
        BehaviorTree tree = Instantiate(this);
        tree.RootNode = tree.RootNode.Clone();
        tree.Nodes = new List<Node>();
        Traverse(tree.RootNode, (n) =>
        {
            tree.Nodes.Add(n);
        });

        return tree;
    }

    public void Bind(NPCController controller)
    {
        Traverse(RootNode, node =>
        {
            node.NPC = controller;
            node.Blackboard = Blackboard;
        });
    }
}
