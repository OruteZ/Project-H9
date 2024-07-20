// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using BT;
// using UnityEngine;
//
// public interface INode
// {
//     public enum ENodeState
//     {
//         Running,
//         Success,
//         Failure
//     }
//     ENodeState Evaluate();
//     
//     public static INode GetNode(NodeInfo info, BehaviourTree tree)
//     {
//         if (info.node is not BTNodeType.Selector and not BTNodeType.Sequence)
//         {
//             return info.node switch
//             {
//                 BTNodeType.IsOutOfAmmo => new IsOutOfAmmo(tree),
//                 BTNodeType.IsPlayerOutOfRange => new IsPlayerOutOfRange(tree),
//                 BTNodeType.IsPlayerOutOfSight => new IsPlayerOutOfSight(tree),
//                 BTNodeType.FinishTurn => new FinishTurn(tree),
//                 BTNodeType.TryReload => new TryReload(tree),
//                 BTNodeType.TryShootPlayer => new TryShootPlayer(tree),
//                 BTNodeType.TryMoveOneTileToPlayer => new TryMoveOneTileToPlayer(tree),
//                 _ => throw new ArgumentOutOfRangeException()
//             };
//             
//             
//         }
//
//         List<INode> children = 
//             info.
//             children.
//             Select(child => GetNode(child, tree)).ToList();
//
//         INode result = info.node switch
//         {
//             BTNodeType.Selector => new SelectorNode(children),
//             BTNodeType.Sequence => new SequenceNode(children),
//             _ => null
//         };
//         
//         return result;
//     }
// }
//
// [Serializable]
// public struct NodeInfo
// {
//     public BTNodeType node;
//     public List<NodeInfo> children;
// }
//
// public enum BTNodeType
// {
//     Sequence,
//     Selector,
//     IsOutOfAmmo,
//     IsPlayerOutOfRange,
//     IsPlayerOutOfSight,
//     FinishTurn,
//     TryReload,
//     TryShootPlayer,
//     TryMoveOneTileToPlayer,
// }