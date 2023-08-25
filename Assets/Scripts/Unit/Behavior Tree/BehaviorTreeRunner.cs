using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Tree {number}", menuName = "ScriptableObjects/AiBT", order = 1)]
public class BehaviorTreeRunner : ScriptableObject
{
    [SerializeField]
    private readonly INode _rootNode;
    public BehaviorTreeRunner(INode rootNode)
    {
        _rootNode = rootNode;
    }

    public void Operate()
    {
        _rootNode.Evaluate();
    }
    
    [ContextMenu("Print Tree Info")]
    public void PrintTree()
    {
        Queue<INode> container = new Queue<INode>();
        container.Enqueue(_rootNode);

        while (container.TryDequeue(out var current))
        {
            string line = "";

            switch (current)
            {
                case SelectorNode curSelector:
                {
                    line = "Selector";
                    foreach(var node in curSelector.GetChildren())
                    {
                        container.Enqueue(node);
                    }

                    break;
                }
                case SequenceNode curSequence:
                {
                    line = "Sequence";
                    foreach(var node in curSequence.GetChildren())
                    {
                        container.Enqueue(node);
                    }

                    break;
                }
                case ActionNode curAction:
                    line = curAction.GetFunc().ToString();
                    break;
            }

            Debug.Log(line);
        }
    }
}
