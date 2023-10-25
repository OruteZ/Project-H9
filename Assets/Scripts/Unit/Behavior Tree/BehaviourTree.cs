using System;
using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Behavior Tree {number}", menuName = "ScriptableObjects/AiBT", order = 1)]
public class BehaviourTree : ScriptableObject
{
    [SerializeField] 
    public NodeInfo nodeInfo;
    
    private INode _rootNode;
    private Unit _unit;
    public Vector3Int playerPosMemory;

    public UnityEvent onSetUp = new UnityEvent();

    public void Setup()
    {
        _rootNode = INode.GetNode(nodeInfo, this);
        onSetUp.Invoke();
    }

    public void Operate(Unit unit)
    {
        _unit = unit;
        _rootNode.Evaluate();
    }
    
    [ContextMenu("Print Tree Info")]
    public void PrintTree()
    {
        void PrintNode(INode node, int depth)
        {
            string log = "";
            for (int i = 0; i < depth; i++) log += "    ";
            log += node;
            Debug.Log(log);
            
            if (node is SelectorNode selector)
            {
                foreach (INode child in selector.GetChildren())
                {
                    PrintNode(child, depth + 1);
                }

                return;
            }
            if (node is SequenceNode sequence)
            {
                foreach (INode child in sequence.GetChildren())
                {
                    PrintNode(child, depth + 1);
                }
            }
        }

        PrintNode(_rootNode, 0);
    }

    [ContextMenu("Set Basic Tree")]
    private void SetBasic()
    {
        _rootNode = new SelectorNode(new List<INode>
        {
            new SequenceNode(new List<INode>
            {
                new IsOutOfAmmo(this),
                new TryReload(this)
            }),

            //-----------

            new SequenceNode(new List<INode>
            {
                new IsPlayerOutOfSight(this),
                new TryMoveOneTileToPlayer(this),
            }),

            //------------

            new SequenceNode(new List<INode>
            {
                new IsPlayerOutOfRange(this),
                new TryMoveOneTileToPlayer(this)
            }),

            //------------

            new TryShootPlayer(this),
            new TryMoveOneTileToPlayer(this),
            new FinishTurn(this)
        });
    }

    public Unit GetUnit() => _unit;
}
