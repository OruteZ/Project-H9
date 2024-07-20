using System.Collections;
using UnityEngine;

public class CoverAction : BaseAction
{
    #region FIELDS
    
    private Vector3Int _targetPos;
    private bool _animationDone;
    
    #endregion
    
    public override ActionType GetActionType()
    {
        return ActionType.Cover;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _targetPos = targetPos;
    }

    public override bool CanExecute()
    {
        var tile = FieldSystem.tileSystem.GetTile(_targetPos);
        
        if (tile is null)
        {
            Debug.LogWarning("target tile is null");
            return false;
        }
        
        if (tile.GetTileObject<CoverableObj>() is null)
        {
            Debug.LogWarning("target tile has no cover");
            return false;
        }
        
        return true;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        var tile = FieldSystem.tileSystem.GetTile(targetPos);
        
        if (tile is null)
        {
            Debug.LogWarning("target tile is null");
            return false;
        }
        
        if (tile.GetTileObject<CoverableObj>() is null)
        {
            Debug.LogWarning("target tile has no cover");
            return false;
        }
        
        return true;
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

   

    protected override IEnumerator ExecuteCoroutine()
    {
        // unit . cover
        
        _animationDone = true;
        // play animation
        
        // unit.animator.SetTrigger(COVER);
        
        CoverableObj coverObj = FieldSystem.tileSystem.GetTile(_targetPos).GetTileObject<CoverableObj>();
        if (coverObj == null)
        {
            throw new System.Exception(
                "CoverObj Component is null"
                + " at " + _targetPos);
        }
        
        coverObj.SetUnit(unit);
        
        // wait until animation is done
        yield return new WaitUntil(() => _animationDone);
        
        // end turn
        unit.EndTurn();
    } 
    
    public override void TossAnimationEvent(string eventString)
    {
        if (eventString == AnimationEventNames.COVER)
        {
            _animationDone = true;
        }
    }

    protected override void SetAmount(float[] amounts)
    {
        base.SetAmount(amounts);
        
    }
}