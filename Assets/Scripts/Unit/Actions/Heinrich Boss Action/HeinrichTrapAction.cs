using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HeinrichTrapAction : BaseAction
{
    [SerializeField] private GameObject trapPrefab;
    
    [Header("Trap Settings")]
    [SerializeField] private int damage;
    [SerializeField] private int boundDuration;
    [SerializeField] private int coolDown;
    [SerializeField] private int maxTrapCount;

    [Space(10)]
    [SerializeField] private float hitrateBuffAmount;
    [SerializeField] private float hitrateBuffDuration;
    
    [Space(5)]
    [SerializeField] private float damageBuffAmount;
    [SerializeField] private float damageBuffDuration;

    private readonly Queue<HeinrichTrap> _traps = new Queue<HeinrichTrap>();
    private int _coolDown;
    
    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        _coolDown = coolDown;
        unit.onTurnEnd.AddListener(OnTurnEnd);
    }

    public override ActionType GetActionType()
    {
        return ActionType.HeinrichTrap;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        return;
    }

    public override bool CanExecute()
    {
        return IsSelectable();
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        int range = unit.stat.sightRange;
        
        // check distance
        if (Hex.Distance(targetPos, unit.hexPosition) > range)
        {
            return false;
        }
        
        // check if trap settable on target tile
        Tile targetTile = FieldSystem.tileSystem.GetTile(targetPos);

        if (targetTile == null) return false;
        if (targetTile.walkable == false) return false;
        if (targetTile.GetTileObject<HeinrichTrap>()) return false;
        if (FieldSystem.unitSystem.GetUnit(targetPos)) return false;
        
        return true;
    }

    public override bool IsSelectable()
    {
        if (_coolDown > 0) return false;
        
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        Tile target = GetRandomTileInRange(unit.stat.sightRange);
        if (target == null)
        {
            Debug.LogError("HeinrichTrapAction: No tile to set trap.");
            yield break;
        }
        
        yield return new WaitForSeconds(1.5f);
        
        bool isSetupFinished = false;
        CreateTrap(unit.hexPosition, () => isSetupFinished = true);
        yield return new WaitUntil(() => isSetupFinished);
        
        _coolDown = coolDown;
    }
    
    private void CreateTrap(Vector3Int targetPos, UnityAction onFinishSetup = null)
    {
        GameObject go = Instantiate(trapPrefab, unit.transform.position, Quaternion.identity);
        if (go.TryGetComponent(out HeinrichTrap trap) is false)
        {
            Debug.LogError("HeinrichTrapAction: trapPrefab does not have HeinrichTrap component.");
            return;
        }
        
        _traps.Enqueue(trap);
        if (_traps.Count > maxTrapCount)
        {
            HeinrichTrap oldestTrap = _traps.Dequeue();
            oldestTrap.RemoveSelf();
        }
        
        trap.SetUp(unit, targetPos, damage, boundDuration, onFinishSetup);
    }
    
    private Tile GetRandomTileInRange(int range)
    {
        List<Tile> tiles = FieldSystem.tileSystem.GetTilesInRange(unit.hexPosition, range).ToList();
        
        List<Tile> trapSettableTiles = tiles.Where(tile =>
        {
            if (tile.walkable == false) return false;
            if (tile.GetTileObject<HeinrichTrap>()) return false;
            if (FieldSystem.unitSystem.GetUnit(tile.hexPosition)) return false;
            
            return true;
        }).ToList();
        
        return trapSettableTiles.Count == 0 ? null : trapSettableTiles[Random.Range(0, trapSettableTiles.Count)];
    }
    
    private void OnTurnEnd(Unit owner)
    {
        _coolDown--;
    }
}