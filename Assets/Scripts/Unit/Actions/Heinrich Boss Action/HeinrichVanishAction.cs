using System.Collections;
using UnityEngine;

public class HeinrichVanishAction : BaseAction
{
    [SerializeField] private int vanishHp;
    [SerializeField] private int moveMinRange;
    
    [Space(10)]
    [SerializeField] private Material originMaterial;
    [SerializeField] private Material vanishMaterial;
    [SerializeField] private bool _tirggered = false;
    
    private Renderer _renderer;

    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        
        _renderer = unit.GetComponentInChildren<Renderer>();
        originMaterial = _renderer.material;
    }

    public override ActionType GetActionType()
    {
        return ActionType.HeinrichVanish;
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
        return IsSelectable();
    }

    public override bool IsSelectable()
    {
        if (unit.stat.curHp > vanishHp) return false;
        if (RandomTile(out _) is false) return false;
        if (_tirggered) return false;

        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        
        if (RandomTile(out Tile tile) is false)
        {
            _tirggered = false;
            Debug.LogError("HeinrichVanishAction: No tile to move.");
            yield break;
        }
        
        //vanish
        SetVanish(true);
        
        
        unit.hexPosition = tile.hexPosition;
        unit.transform.position = tile.transform.position;
        _tirggered = true;
        
        // event 
        unit.onHit.AddListener(OnHit);
    }

    private void OnHit(Damage arg0)
    {
        if (arg0.Contains(Damage.Type.UNVANISHABLE) is false) return;
        
        RemoveVanish();
        
        unit.onHit.RemoveListener(OnHit);
    }

    private bool RandomTile(out Tile tile)
    {
        // all tile list
        var allTiles = FieldSystem.tileSystem.GetAllTiles();
        
        // remove impassable tiles
        allTiles.RemoveAll(tile =>
        {
            if (tile.walkable == false) return true;
            if (FieldSystem.unitSystem.GetUnit(tile.hexPosition)) return true;
            if (Hex.Distance(unit.hexPosition, tile.hexPosition) <= moveMinRange) return true;
            if (tile.GetTileObject<HeinrichTrap>()) return true;

            return false;
        });
        
        // if no tile to move
        if (allTiles.Count == 0)
        {
            tile = null;
            return false;
        }
        
        // return random tile
        tile = allTiles[Random.Range(0, allTiles.Count)];
        return true;
    }

    private void SetVanish(bool b)
    {
        unit.vanishTrigger = b;
    }
    
    private void RemoveVanish()
    {
        SetVanish(false);
        
        // get player
        var player = FieldSystem.unitSystem.GetPlayer();
        //reload sight
        player.ReloadSight();
    }
}