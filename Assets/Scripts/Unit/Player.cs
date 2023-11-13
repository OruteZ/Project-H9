using System.Collections;
using System.Collections.Generic;
using Generic;
using PassiveSkill;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : Unit
{
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    public override void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(newName, unitStat, newWeapon, unitModel, passiveList);
        
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnChanged);
        TileEffectManager.instance.SetPlayer(this);

        onSelectedChanged.AddListener(() => UIManager.instance.onActionChanged.Invoke());
        FieldSystem.onStageAwake.AddListener(ReloadSight);
    }
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (UIManager.instance.isMouseOverUI) return;
        if (HasStatus(StatusEffectType.Stun)) FieldSystem.turnSystem.EndTurn(); 

        var isMouseOnTile = TryGetMouseOverTilePos(out var onMouseTilePos);

        if (isMouseOnTile && GetSelectedAction().GetActionType() is
                ActionType.Attack or ActionType.Fanning)
        {
            var target = FieldSystem.unitSystem.GetUnit(onMouseTilePos);
            if (target is not Player and not null)
            {
                transform.LookAt(Hex.Hex2World(onMouseTilePos), Vector3.up);
                // var curRotation = transform.localRotation.eulerAngles;
                // curRotation.z = curRotation.x = 0;
                //transform.localRotation = Quaternion.Euler(curRotation);
            }
        }
        
        
        if (Input.GetMouseButtonDown(0) && isMouseOnTile) 
        {
            SetBusy();

            var actionSuccess = TryExecuteUnitAction(onMouseTilePos);
            Debug.Log("actionSuccess: " + actionSuccess);
            
            if(actionSuccess is false) ClearBusy();
        }

        if (Input.GetMouseButton(1))
        {
            SelectAction(GetAction<IdleAction>());
        }
    }

    public override void StartTurn()
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            SelectAction(GetAction<MoveAction>());
            ReloadSight();
        }
    }

    public void EndTurn()
    {
        FieldSystem.turnSystem.EndTurn();
    }

    public void ContinueWorldTurn()
    {
        stat.SetOriginalStat(StatType.CurActionPoint, GameManager.instance.worldAp);
        SelectAction(GetAction<MoveAction>());
    }
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static bool TryGetMouseOverTilePos(out Vector3Int pos)
    {
        if (UIManager.instance.isMouseOverUI)
        {
            pos = Vector3Int.zero;
            return false;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue ,layerMask : LayerMask.GetMask("Tile")))
        {
            var tile = hit.collider.GetComponent<Tile>();

            if (tile is null)
            {
                pos = Vector3Int.zero;
                return false;
            }

            if (GameManager.instance.CompareState(GameState.World))
            {
                pos = tile.hexPosition;
                return true;
            }

            if (tile.inSight)
            {
                pos = tile.hexPosition;
                return true;
            }
        }

        pos = Vector3Int.zero;
        return false;
    }

    public void ReloadSight()
    {
        //한칸 움직일때마다 호출되므로, 보였다가 시야에서 사라지는 경우는 sightRange + 1로 탐색 범위에 포함 시킬 수 있음
        IEnumerable<Tile> allTile =
            FieldSystem.tileSystem.GetTilesInRange(hexPosition, stat.sightRange + 1);

        foreach (var tile in allTile)
        {
            tile.inSight = 
                FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition) &&
                Hex.Distance(hexTransform.position, tile.hexPosition) <= stat.sightRange;

#if UNITY_EDITOR
            var unitVision = FieldSystem.unitSystem.GetUnit(tile.hexPosition);
            if (unitVision != null)
            {
                Debug.Log("Unit : " + unitVision.gameObject.name);
                Debug.Log("Vision Check = " + FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition));
                Debug.Log("Distance = " + Hex.Distance(hexTransform.position, tile.hexPosition));
            }
#endif
        }
    }
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        UIManager.instance.onPlayerStatChanged.Invoke();
        GameManager.instance.playerStat = stat;
    }
    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.isVisible = FieldSystem.tileSystem.VisionCheck(hexPosition, unit.hexPosition) &&
                             Hex.Distance(hexTransform.position, unit.hexPosition) <= stat.sightRange;
        }
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
        foreach (var obj in FieldSystem.tileSystem.GetTile(hexPosition).interactiveObjects) 
        { 
            obj.OnCollision(unit);
        }
    }

    private void OnTurnChanged()
    {
        if(IsMyTurn() is false) SelectAction(GetAction<IdleAction>());
    }
}
