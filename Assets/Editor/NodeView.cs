using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node Node;
    public Port Input;
    public Port Output; 

    public NodeView(Node node): base("Assets/Editor/NodeView.uxml")
    {
        Node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "Description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (Node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (Node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (Node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (Node is RootNode)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if(Node is ActionNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if(Node is CompositeNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if(Node is DecoratorNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is RootNode)
        {
            
        }

        if (Input != null)
        {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(Input);
        }
    }

    private void CreateOutputPorts()
    {
        if (Node is ActionNode)
        {
            
        }
        else if (Node is CompositeNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (Node is DecoratorNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is RootNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (Output != null)
        {
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(Output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(Node, "Behavior Tree (Set Position)");
        Node.Position.x = newPos.xMin;
        Node.Position.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        CompositeNode composite = Node as CompositeNode;
        if (composite)
        {
            composite.Children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.Position.x < right.Position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (Node.State)
            {
                case Node.StateEnum.Running:
                    if (Node.Started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case Node.StateEnum.Failure:
                    AddToClassList("failure");
                    break;
                case Node.StateEnum.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
