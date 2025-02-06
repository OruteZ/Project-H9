using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : TileObject
{ 
    /// <summary>
    /// 해당 Action들을 취하면 Bush안이어도 Unit이 보이게 됩니다.
    /// </summary>
    private static readonly ActionType[] visibleActions = {
        ActionType.Attack,
        ActionType.Dynamite,
        ActionType.Fanning,
        ActionType.SpreadDynamite,
        ActionType.ItemUsing
    };
   
    
    public override string[] GetArgs()
    { return new string[] { }; }
    
    public override void SetArgs(string[] args)
    { return; }
    
    private void OnStartAction(IUnitAction a, Vector3Int target)
    {
        Debug.Log("EvOnStartAction");
        
        bool contains = Array.Exists(
            visibleActions, 
            element => element == a.GetActionType()
            );
        if (contains)
        {
            BushSystem.Instance.AddNonHideUnit(a.GetUnit());
        }
        return;
    }
    
    private void OnFinishAction(IUnitAction action)
    {
        Debug.Log("EvOnFinishAction");
        BushSystem.Instance.RemoveNonHideUnit(action.GetUnit());
    }
    
    private void LinkEvents(Unit unit)
    {
        unit.onFinishAction.AddListener(OnFinishAction);
        unit.onActionStart.AddListener(OnStartAction);
    }
    
    private void UnlinkEvents(Unit unit)
    {
        unit.onFinishAction.RemoveListener(OnFinishAction);
        unit.onActionStart.RemoveListener(OnStartAction);
    }

    public override void OnHexCollisionEnter(Unit other)
    {
        LinkEvents(other);
        return;
    }
    
    public override void OnHexCollisionExit(Unit other)
    {
        UnlinkEvents(other);
        return;
    }
    
    public override void SetUp()
    {
        base.SetUp();
        
        tile.visible = false;
        tile.walkable = true;
        tile.rayThroughable = true;
        
        BushSystem.Instance.AddBush(tile.hexPosition, this, true);
    }
    
    
    #region VISION & MESHRENDERER
    // =========== multi mesh renderer object이기 때문에 이렇게 처리함 ===========
    [Header("Vision")]
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private bool bushVision;
    
    protected override void InitRenderer()
    {
        // get children mesh renderers
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }
    
    public override bool IsVisible()
    {
        return bushVision;
    }
    
    public override void SetVisible(bool value)
    {
        bushVision = value;
        foreach (MeshRenderer r in meshRenderers)
        {
            r.enabled = value;
        }
    }
    
    #endregion

    /// <summary>
    /// 한칸 이내의 근처 Bush를 찾아서 반환합니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Bush> GetNeighbors()
    {
        foreach (var dir in Hex.directions)
        {
            Vector3Int neighborPos = tile.hexPosition + dir;
            List<TileObject> objs = FieldSystem.tileSystem.GetTileObject(neighborPos);
            foreach (TileObject obj in objs)
            {
                if (obj is Bush bush)
                {
                    yield return bush;
                }
            }
        }
    }
}