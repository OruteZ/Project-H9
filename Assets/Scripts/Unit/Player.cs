using System.Collections;
using System.Collections.Generic;
using Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : Unit
{
    [HideInInspector] public UnityEvent onSelectedChanged;
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    public override void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel)
    {
        base.SetUp(newName, unitStat, newWeapon, unitModel);
        
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnChanged);
        TileEffectManager.instance.SetPlayer(this);

        onSelectedChanged.AddListener(() => UIManager.instance.combatUI.SetCombatUI());
    }
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (UIManager.instance.isMouseOverUI) return;

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

            var actionSuccess = TryExecuteUnitAction(onMouseTilePos, FinishAction);
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
#if UNITY_EDITOR
        Debug.Log("Player Turn Started");
#endif
        
        hasAttacked = false;
        currentActionPoint = stat.actionPoint;
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            animator.SetTrigger(START_TURN);
            SelectAction(GetAction<IdleAction>());
        }
        else
        {
            SelectAction(GetAction<MoveAction>());
            ReloadSight();
        }
    }

    public void ContinueWorldTurn()
    {
        currentActionPoint = GameManager.instance.worldAp;
        SelectAction(GetAction<MoveAction>());
    }
    
    public void SelectAction(IUnitAction action)
    {
        if (IsBusy()) return;
        if (IsMyTurn() is false) return;
        if (action.IsSelectable() is false) return;
        if (action.GetCost() > currentActionPoint)
        {  
            Debug.Log("Cost is loss, Cost is " + action.GetCost());
            return;
        }
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif

        activeUnitAction = action;
        onSelectedChanged.Invoke();

        if (activeUnitAction.CanExecuteImmediately())
        { 
            if (activeUnitAction is not IdleAction) SetBusy();
            var actionSuccess = TryExecuteUnitAction(Vector3Int.zero, FinishAction);
            Debug.Log("actionSuccess: " + actionSuccess);
            
            if(actionSuccess is false) ClearBusy();
        }
    }

    private void FinishAction()
    {
        Debug.Log("Finish Action : player");
        if(activeUnitAction is not MoveAction) ConsumeCost(activeUnitAction.GetCost());
        
        ClearBusy();
        if(GameManager.instance.CompareState(GameState.Combat))
        {
            var idleAction = GetAction<IdleAction>();
            SelectAction(idleAction is null ? GetAction<MoveAction>() : idleAction);
        }
        else
        {
            SelectAction(GetAction<IdleAction>());
            SelectAction(GetAction<MoveAction>());
        }
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
    
    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);

        GameManager.instance.playerStat = stat;
    }

    private void LookTarget()
    {
        
    }

    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.isVisible = FieldSystem.tileSystem.VisionCheck(hexPosition, unit.hexPosition) &&
                             Hex.Distance(hexTransform.position, unit.hexPosition) <= stat.sightRange;
        }
        
//        Debug.Log("On Any Unit Moved : Invoke");
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
