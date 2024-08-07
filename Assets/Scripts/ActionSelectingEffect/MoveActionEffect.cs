using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveActionEffect : BaseSelectingActionEffect
{
    
    private Dictionary<Vector3Int, GameObject> _baseEffect;
    private Dictionary<Vector3Int, GameObject> _dynamicEffect;

    private Unit _unit;
    private Coroutine _dynamicCoroutine;
    
    //=============================MAIN============================
    public override void StopEffect()
    {
        _baseEffect.Values.ToList().ForEach(Destroy);
        _dynamicEffect.Values.ToList().ForEach(Destroy);
        
        _baseEffect.Clear();
        _dynamicEffect.Clear();
        effector.StopCoroutine(_dynamicCoroutine);
    }

    public override void ShowEffect(Unit user)
    {
        _unit = user;
        
        // setting user's walkable Tiles
        int range = GetMovableRange();
        var tiles = FieldSystem.tileSystem.GetWalkableTiles(_unit.hexPosition, range);

        // Try Set Base Tile
        foreach (Tile tile in tiles)
        {
            TrySetBaseTile(tile);
        }

        _dynamicCoroutine = effector.StartCoroutine(DynamicCoroutine());
    }

    private IEnumerator DynamicCoroutine()
    {
        int range = GetMovableRange();

        int prevRouteLength = -1;
        
        while (true)
        {
            bool success = Player.TryGetMouseOverTilePos(out Vector3Int target);
            if (success is false)
            {
                // fail to get mouse over tile
                yield return ReloadDynamicCo(Array.Empty<Vector3Int>());
                continue;
            }
            
            // check movable : tile's walkable, visible, is there unit
            bool isThereUnit = FieldSystem.unitSystem.GetUnit(target) is not null;
            bool isVisibleTile = FieldSystem.tileSystem.GetTile(target).visible;
            bool isWalkableTile = FieldSystem.tileSystem.GetTile(target).walkable;
            if (isThereUnit || !isVisibleTile || !isWalkableTile)
            {
                // fail to get mouse over tile
                yield return ReloadDynamicCo(Array.Empty<Vector3Int>());
                continue;
            }
                
            List<Tile> route = FieldSystem.tileSystem.FindPath(_unit.hexPosition, target);
            if (route is null)
            {
                // fail to find path
                yield return ReloadDynamicCo(Array.Empty<Vector3Int>());
                continue;
            }
            
            // todo : refactor UI Manager
            // check routePath
            if (prevRouteLength != route.Count)
            {
                UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage = route.Count - 1;
                UIManager.instance.onPlayerStatChanged.Invoke();
            }
            prevRouteLength = route.Count;

            // route contains start position
            // so if range is over than routeCount clear effect
            if(route.Count - 1 <= range)
            {
                yield return ReloadDynamicCo(route.Select(tile => tile.hexPosition));
            }
            else
            {
                yield return ReloadDynamicCo(Array.Empty<Vector3Int>());
            }
        }
        
        // Resharper disable one IteratorNeverReturns
    }
    
    //===============================================================

    protected override void OnSetup()
    {
        _baseEffect = new Dictionary<Vector3Int, GameObject>();
        _dynamicEffect = new Dictionary<Vector3Int, GameObject>();
    }

    private int GetMovableRange()
    {
        int range = 0;
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            range = _unit.stat.curActionPoint / _unit.GetAction<MoveAction>().GetCost();
        }
        else
        {
            range = _unit.stat.sightRange;
        }

        return range;
    }
    
    private bool TrySetBaseTile(Tile tile)
    {
        // null checking
        if (tile is null)
        {
            Debug.LogError("Method GetWalkableTiles : return value contains null tile");
            return false;
        }
            
        // exception handling 1 : target that is out of sight
        int dist = Hex.Distance(tile.hexPosition, _unit.hexPosition);
        int sightRange = _unit.stat.sightRange;
        if (dist > sightRange) return false;

        // exception handing 2 : vision check is false (ONLY IN COMBAT)
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            bool visible = FieldSystem.tileSystem.VisionCheck(
                _unit.hexPosition,
                tile.hexPosition
            );

            if (visible is false) return false;
        }
        
        // set tile
        InstantiateTile(true, tile.hexPosition);
        return true;
    }

    private void InstantiateTile(bool isBase, Vector3Int hex)
    {
        // get value from setting
        GameObject defaultTile = setting.tileEffectDefault;
        Color color = isBase ? setting.movableTileColor : setting.routeColor;
        
        // instantiate
        Vector3 spawnPoint = Hex.Hex2World(hex);
        spawnPoint.y += isBase ? 0.02f : 0.03f;
        GameObject instance = Instantiate(defaultTile, spawnPoint, Quaternion.identity);

        // Setting Color
        if (instance.TryGetComponent(out Renderer renderer) is false)
        {
            Material matCopy = new (renderer.material);
            matCopy.color = color;
            renderer.material = matCopy;
        }
        else
        {
            Debug.LogError("Default Tile GameObject dost not have Renderer Component");
        }
        
        // Set into Dict
        if (isBase) _baseEffect.Add(hex, instance);
        else _dynamicEffect.Add(hex, instance);
    }

    private IEnumerator ReloadDynamicCo(IEnumerable<Vector3Int> hexList)
    {
        // remove that is not in hexList
        IEnumerable<Vector3Int> toRemove = _dynamicEffect.Keys.ToList().Where(g => ! hexList.Contains(g));
        foreach(Vector3Int target in toRemove)
        {
            Destroy(_dynamicEffect[target]);
            _dynamicEffect.Remove(target);
        }
        
        // create new
        IEnumerable<Vector3Int> toInstantiate = hexList.Where(h => ! _dynamicEffect.ContainsKey(h));
        foreach (Vector3Int target in toInstantiate)
        {
            InstantiateTile(false, target);
        }

        yield return null;
    }
}