// using System;
// using System.Collections.Generic;
// using Generic;
// using Unity.VisualScripting;
// using UnityEngine;
//
// // ReSharper disable once CheckNamespace
// namespace PlayerState
// {
//     public class WaitState : State<Player>
//     {
//         public override void Enter(Player entity)
//         { }
//         public override void Execute(Player entity)
//         { }
//         public override void Exit(Player entity)
//         { }
//     }
//     
//     public class SelectState : State<Player>
//     {
//         public override void Enter(Player entity)
//         {
//             //Player Control UI ON
//         }
//         public override void Execute(Player entity)
//         { }
//
//         public override void Exit(Player entity)
//         {
//             //Player Control UI Off
//         }
//     }
//
//     public class Move : State<Player>, IActionState 
//     {
//         private const float OneTileMoveTime = 0.2f;
//         private const float OneDivideOneTileMoveTime = 1 / 0.2f;
//
//         private IEnumerable<Tile> _tilesInRange;
//         private IEnumerable<Tile>  _walkableTiles;
//
//         private Vector3Int _current;
//         private Vector3Int _next;
//         private List<Tile> _path;
//
//         private float _time;
//         private int _pathIndex;
//         
//         private Move(Player entity)
//         {
//             var mobility = entity.CalculateMobility();
//             
//             var map = entity.hexTransform.Map;
//             _tilesInRange = map.GetTilesInRange(entity.hexTransform.position, mobility);
//             _walkableTiles = map.GetWalkableTiles(entity.hexTransform.position, mobility);
//         }
//
//         public override void Enter(Player entity)
//         {
//             Debug.Log("Current State = Move");
//             
//             _path = (List<Tile>)
//                 entity.hexTransform.Map.FindPath(
//                     entity.Position, 
//                     entity.target.position);
//             _pathIndex = 0;
//             _next = _path[0].position;
//             _time = 0;
//         }
//
//         public override void Execute(Player entity)
//         {
//             if (_time >= OneTileMoveTime)
//             {
//                 if (++_pathIndex >= _path.Count)
//                 {
//                     entity.ChangeState(new SelectState());
//                 }
//                 
//                 _current = _next;
//                 _next = _path[++_pathIndex].position;
//
//                 entity.hexTransform.position = _current;
//                 _time = 0;
//             }
//             else
//             {
//                 entity.transform.position =
//                     Vector3.Lerp(Hex.Hex2World(_current),
//                         Hex.Hex2World(_next),
//                         _time * OneDivideOneTileMoveTime);
//             }
//             _time += Time.deltaTime;
//         }
//
//         public override void Exit(Player entity)
//         {
//             entity.actionPoint -= _path.Count - 1;
//             GC.Collect();
//         }
//
//
//         void IActionState.OnSelect()
//         {
//             Debug.Log("TileEffector에 이동가능 범위 표시 효과 적용");
//             TileEffector.Instance.NormalEffect(_walkableTiles);
//         }
//
//         void IActionState.OffSelect()
//         {
//             Debug.Log("TileEffector 초기화");
//         }
//     }
//
// }
