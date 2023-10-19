// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting.Dependencies.NCalc;
// using UnityEngine;
//
// public class EnemyAI : MonoBehaviour
// {
//     private HexTransform _hexTransform;
//     private Enemy _enemy;
//
//     [SerializeField]
//     public Vector3Int playerPosMemory;
//
//     public IUnitAction resultAction;
//     public Vector3Int resultPosition;
//     
//     
//     [SerializeField] private BehaviourTree _tree;
//
//     private void Awake()
//     {
//         _hexTransform = GetComponent<HexTransform>();
//         _enemy = GetComponent<Enemy>();
//     }
//     
//     public void Start() 
//     {
//         playerPosMemory = FieldSystem.unitSystem.GetPlayer().hexPosition;
//     }
//     
//     public void SelectAction()
//     {
//         _tree.Operate();
//     }
//
//     private INode.ENodeState IsPlayerOutOfSight()
//     {
//         if (FieldSystem.unitSystem.GetPlayer() is null) return INode.ENodeState.Success;
//         
//         var curPlayerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
//         if (FieldSystem.tileSystem.VisionCheck(_enemy.hexPosition, curPlayerPos))
//         {
//             playerPosMemory = curPlayerPos;
//             
//             Debug.Log("AI Think : Player is in sight");
//             
//             return INode.ENodeState.Failure;
//         }
//         
//         
//         Debug.Log("AI Think : Player is out of sight");
//         return INode.ENodeState.Success;
//     }
//
//     private INode.ENodeState IsOutOfAmmo()
//     {
//         return INode.ENodeState.Failure;
//     }
//
//
//     private INode.ENodeState IsOutOfRange()
//     {
//         if (_enemy.weapon.GetRange() < Hex.Distance(playerPosMemory, _enemy.hexPosition))
//         {
//             Debug.Log("AI Think player is out of range");
//             return INode.ENodeState.Success;
//         }
//         else
//         {
//             
//             Debug.Log("AI Think player is in range");
//             return INode.ENodeState.Failure;
//         }
//     }
// }
