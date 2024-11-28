// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// public class CoverAction : BaseAction
// {
//     #region FIELDS
//     
//     private Vector3Int _targetPos;
//     private bool _animationDone;
//     private static readonly int COVER = Animator.StringToHash("Cover");
//
//     #endregion
//     
//     public override ActionType GetActionType()
//     {
//         return ActionType.Cover;
//     }
//
//     public override void SetTarget(Vector3Int targetPos)
//     {
//         _targetPos = targetPos;
//     }
//
//     public override bool CanExecute()
//     {
//         // var tile = FieldSystem.tileSystem.GetTile(_targetPos);
//         //
//         // if (tile is null)
//         // {
//         //     Debug.LogWarning("target tile is null");
//         //     return false;
//         // }
//         //
//         // if (tile.GetTileObject<CoverableObj>() is null)
//         // {
//         //     Debug.LogWarning("target tile has no cover");
//         //     return false;
//         // }
//         //
//         // return true;
//
//         return false;
//     }
//
//     public override bool CanExecute(Vector3Int targetPos)
//     {
//         // var tile = FieldSystem.tileSystem.GetTile(targetPos);
//         //
//         // if (tile is null)
//         // {
//         //     Debug.LogWarning("target tile is null");
//         //     return false;
//         // }
//         //
//         // if (tile.GetTileObject<CoverableObj>() is null)
//         // {
//         //     Debug.LogWarning("target tile has no cover");
//         //     return false;
//         // }
//         //
//         return false;
//     }
//
//     public override bool IsSelectable()
//     {
//         // if there is a coverable object in the target tile
//         // IEnumerable<Tile> adjustTiles = 
//         //     FieldSystem.tileSystem.GetTilesInRange(unit.hexPosition, 1);
//         //
//         // return adjustTiles.Any(tile => tile.GetTileObject<CoverableObj>() != null);
//
//         return false;
//     }
//
//     public override bool CanExecuteImmediately()
//     {
//         return false;
//     }
//
//    
//
//     protected override IEnumerator ExecuteCoroutine()
//     {
//         // unit . cover
//         // play animation
//         
//         transform.LookAt(FieldSystem.tileSystem.GetTile(_targetPos).transform);
//         transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
//         
//         
//         unit.animator.SetBool(COVER, true);
//         
//         CoverableObj coverObj = FieldSystem.tileSystem.GetTile(_targetPos).GetTileObject<CoverableObj>();
//         if (coverObj == null)
//         {
//             throw new System.Exception(
//                 "CoverObj Component is null"
//                 + " at " + _targetPos);
//         }
//         
//         coverObj.SetUnit(unit);
//         
//         // wait until animation is done
//         yield return new WaitUntil(() => _animationDone);
//         
//         // end turn
//         unit.EndTurn();
//     } 
//     
//     public override void TossAnimationEvent(string eventString)
//     {
//         if (eventString == AnimationEventNames.COVER)
//         {
//             _animationDone = true;
//         }
//     }
//
//     protected override void SetAmount(float[] amounts)
//     {
//         base.SetAmount(amounts);
//         
//     }
// }