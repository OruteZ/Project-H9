// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public abstract class Acting
// {
//     protected Unit unit;
//
//     public bool isActFinished;
//     public abstract void Execute(Tile target);
//     public abstract void OnSelect(Unit a);
//     public int cost;
// }
//
// public class Moving : Acting
// {
//     private const float OneTileMoveTime = 0.2f;
//
//     public override void Execute(Tile target)
//     {
//         if (!isActFinished) return;
//         IEnumerator MoveCoroutine(IEnumerable<Tile> route)
//         {
//             const float oneDivTileMoveTime = 1 / 0.2f;
//
//             foreach (var dest in route)
//             {
//                 if (dest.hexTransform.position == unit.hexTransform.position) continue;
//             
//                 var start = Hex.Hex2World(unit.hexTransform.position);
//                 var end = Hex.Hex2World(dest.hexTransform.position);
//                 var time = 0f;
//                 while (time <= OneTileMoveTime)
//                 {
//                     time += Time.deltaTime;
//                     unit.transform.position = Vector3.Lerp(start, end, time * oneDivTileMoveTime);
//                     yield return null;
//                 }
//
//                 unit.hexTransform.position = dest.hexTransform.position;
//             }
//         }
//         
//         var route = unit.world.FindPath(unit.hexTransform.position, target.hexTransform.position);
//         if (route == null) return;
//         
//         unit.StartCoroutine(MoveCoroutine(route));
//     }
//
//     public override void OnSelect(Unit unit)
//     {
// #if UNITY_EDITOR
//         Debug.Log("Selected Moving Mode");
// #endif
//         //throw new System.NotImplementedException();
//         this.unit = unit;
//         isActFinished = false;
//     }
// }
//
// public class Attacking : Acting
// {
//     public override void Execute(Tile target) {}
//
//     public override void OnSelect(Unit a)
//     {
//         unit = a;
//     }
//}