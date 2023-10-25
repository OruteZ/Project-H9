using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace BT
{
    public class IsPlayerOutOfSight : ActionNode
    {
        public IsPlayerOutOfSight(BehaviourTree tree) : base(tree)
        {
        }

        public override INode.ENodeState Evaluate()
        {
            if (FieldSystem.unitSystem.GetPlayer() is null) return INode.ENodeState.Success;
            Enemy enemy = (Enemy)tree.GetUnit();
            ref var playerPosMemory = ref tree.playerPosMemory;
            
            var curPlayerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
            if (FieldSystem.tileSystem.VisionCheck(enemy.hexPosition, curPlayerPos))
            {
                // if (Hex.Distance(enemy.hexPosition, curPlayerPos) > enemy.stat.sightRange)
                // {
                //     Debug.Log("AI Think : Player is out of sight");
                //     return INode.ENodeState.Success;
                // }
                playerPosMemory = curPlayerPos;
            
                Debug.Log("AI Think : Player is in sight");
            
                return INode.ENodeState.Failure;
            }
        
        
            Debug.Log("AI Think : Player is out of sight");
            return INode.ENodeState.Success;
        }
    }

    public class IsPlayerOutOfRange : ActionNode
    {
        public IsPlayerOutOfRange(BehaviourTree tree) : base(tree)
        {
        }

        public override INode.ENodeState Evaluate()
        {
            Unit unit = tree.GetUnit();
            if (unit.weapon.GetRange() < Hex.Distance(unit.hexPosition, tree.playerPosMemory))
            {
                return INode.ENodeState.Success;
            }

            return INode.ENodeState.Failure;
        }
    }

    public class IsOutOfAmmo : ActionNode
    {
        public IsOutOfAmmo(BehaviourTree tree) : base(tree)
        {}

        public override INode.ENodeState Evaluate()
        {
            Unit unit = tree.GetUnit();
            
            Debug.Log("AI Think : Ammo is " + unit.weapon.currentAmmo);
            return unit.weapon.currentAmmo is 0 ? INode.ENodeState.Success : INode.ENodeState.Failure;
        }
    }

}