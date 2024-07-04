using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Pool;

public enum TileEffectType
{
    None = 0,
    Normal = 1,
    Friendly = 2,
    Hostile = 3,
    Impossible = 4,
    Invisible = 5,
    Ammunition = 10,
    Saloon = 11,
    Sheriff = 12,
}

/// <summary>
/// 타일을 넘겨받으면 원하는 이펙트를 타일에 적용시켜주는 클래스
/// </summary>
public class TileEffectManager : Generic.Singleton<TileEffectManager>
{
    public GameObject friendlyEffect;
    public GameObject hostileEffect;
    public GameObject normalEffect;
    public GameObject impossibleEffect;
    public GameObject invisibleEffect;

    public GameObject ammunitionEffect;
    public GameObject salonEffect;
    public GameObject sheriffEffect;

    private Player _player;
    private Dictionary<Vector3Int, GameObject> _effectsBase;
    private Dictionary<Vector3Int, GameObject> _effectsRelatedTarget;

    private IObjectPool<GameObject> _baseEffectPool;
    private IObjectPool<GameObject> _targetEffectPool;

    private Coroutine _curCoroutine;

    public Material combatFowMaterial;

    [field: Header("Attack Effect")] 
    public GameObject attackTileEffect;
    public GameObject attackOutOfRangeEffect;
    public GameObject attackUnitEffect;
    public GameObject attackSweetSpotEffect;
    public GameObject attackSweetSpotUnitEffect;
    public RectTransform aimEffectRectTsf;
    
    public RectTransform combatCanvas;
    public RectTransform aimEffect;

    [field: Header("Cover Effect")] 
    public GameObject coverPosition;
    public GameObject coverAvailable;
    

    public void SetPlayer(Player p)
    {
        _player = p;
        p.onSelectedChanged.AddListener(TileEffectSet);
        p.onBusyChanged.AddListener(() =>
        {
            if (_player.IsBusy())
            {
                ClearEffect();
                
            }
            else TileEffectSet();
        });
    }

    #region PRIVATE
    private void TileEffectSet()
    {
        Debug.Log("Setting Tile Effect");
        
        ClearEffect();
        switch (_player.GetSelectedAction().GetActionType())
        {
            case ActionType.Move:
                MovableTileEffect();
                break;
            case ActionType.Spin:
                break;
            case ActionType.Attack:
                AttackTileEffect();
                break;
            case ActionType.Dynamite:
                DynamiteTileEffect();
                break;
            case ActionType.Idle:
                ClearEffect(_effectsBase);
                ClearEffect(_effectsRelatedTarget);
                break;
            case ActionType.Reload:
                break;
            case ActionType.Fanning:
                AttackTileEffect();
                break;
            case ActionType.None:
                break;
            case ActionType.Hemostasis:
                break;
            case ActionType.ItemUsing:
                ItemTileEffect();
                break;
            case ActionType.Cover:
                CoverEffect();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    

    #region MOVE
    private void MovableTileEffect()
    {
        //if world state, get sight range, else get action po
        int range = 0;
        if(GameManager.instance.CompareState(GameState.Combat))
            range = _player.stat.curActionPoint / _player.GetAction<MoveAction>().GetCost();
        else
            range = _player.stat.sightRange;
        
        Vector3Int start = _player.hexPosition;

        var tiles = FieldSystem.tileSystem.GetWalkableTiles(start, range);
        foreach (Tile tile in tiles)
        {
            if (tile is null)
            {
                Debug.LogError("Tile is null");
                continue;
            }
            
            TileEffectType effectType = TileEffectType.Normal;
            if (GameManager.instance.CompareState(GameState.Combat))
            {
                int distance = Hex.Distance(tile.hexPosition, _player.hexPosition);
                int sightRange = _player.stat.sightRange;
                if (distance > sightRange) continue;
                if (!FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition)) continue;
            }
            else //GameState.World
            {
                bool containsFog = tile.tileObjects.OfType<FogOfWar>().Any();
                if (containsFog) continue;
                foreach (Town tileObject in tile.tileObjects.OfType<Town>())
                {
                    effectType = (tileObject).GetTileEffectType();
                }
            }
            SetEffectBase(tile.hexPosition, effectType);
        }

        _curCoroutine = StartCoroutine(MovableTileEffectCoroutine());
    }

    private IEnumerator MovableTileEffectCoroutine()
    {
        int range = 0;
        if(GameManager.instance.CompareState(GameState.Combat))
            range = _player.stat.curActionPoint / _player.GetAction<MoveAction>().GetCost();
        else
            range = _player.stat.sightRange;
        int prevRouteLength = -1;
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.unitSystem.GetUnit(target) is not null)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            var route = FieldSystem.tileSystem.FindPath(_player.hexPosition, target);
            if (route is null)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            if(route.Count - 1 <= range)
            {
                foreach (var pos in route.Select(tile => tile.hexPosition))
                {
                    SetEffectTarget(pos, TileEffectType.Friendly);
                }
            }
            else
            {
                ClearEffect(_effectsRelatedTarget);
            }

            if (prevRouteLength != route.Count)
            {
                UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage = route.Count - 1;
                UIManager.instance.onPlayerStatChanged.Invoke();
            }
            prevRouteLength = route.Count;

        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region SHOOT
    private void AttackTileEffect()
    {
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, _player.weapon.GetRange()).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));
        var units = FieldSystem.unitSystem.units.Where(
            unit => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) && unit is not Player);

        //white range tile
        foreach (var tile in tiles)
        {
            if (FieldSystem.unitSystem.GetUnit(tile.hexPosition) is not null) continue;
            if (Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;

            SetEffectBase(tile.hexPosition, attackTileEffect);
        }

        //target range tile
        foreach (var unit in units)
        {
            if (FieldSystem.tileSystem.RayThroughCheck(_player.hexPosition, unit.hexPosition) is false) continue;
            if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) is false) continue;
            if (Hex.Distance(_player.hexPosition, unit.hexPosition) > _player.stat.sightRange) continue;

            if (_player.weapon.GetRange() >= Hex.Distance(_player.hexPosition, unit.hexPosition))
            {
                SetEffectBase(unit.hexPosition, attackUnitEffect);
            }
            else
            {
                
                if(_player.weapon.GetWeaponType() is not ItemType.Shotgun) 
                    SetEffectBase(unit.hexPosition, attackOutOfRangeEffect);
            }
        }
        
        //if weapon type is repeater && has sweet spot buff, show sweet spot
        if (_player.weapon is Repeater repeater && 
            _player.GetDisplayableEffects().Any(e => e is SweetSpotEffect))
        {
            var sweetSpot = (repeater).GetSweetSpot();
            if (sweetSpot < _player.stat.GetStat(StatType.SightRange))
            {
                //remove tiles in sweet spot range circle
                foreach (var pos in FieldSystem.tileSystem.GetTilesOutLine(_player.hexPosition, sweetSpot))
                {
                    if (_effectsBase.ContainsKey(pos.hexPosition))
                    {
                        Destroy(_effectsBase[pos.hexPosition]);
                        _effectsBase.Remove(pos.hexPosition);
                    }
                
                    SetEffectBase(pos.hexPosition, attackSweetSpotEffect);
                }
            
                //remove units in sweet spot range circle
                foreach (var unit in FieldSystem.unitSystem.units)
                {
                    if (unit is Player) continue;
                    if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) is false) continue;
                    if(Hex.Distance(_player.hexPosition, unit.hexPosition) > _player.stat.sightRange) continue;
                    if (sweetSpot == Hex.Distance(_player.hexPosition, unit.hexPosition))
                    {
                        if (_effectsBase.ContainsKey(unit.hexPosition))
                        {
                            Destroy(_effectsBase[unit.hexPosition]);
                            _effectsBase.Remove(unit.hexPosition);
                        }
                    
                        SetEffectBase(unit.hexPosition, attackSweetSpotUnitEffect);
                    }
                }
            }
        }

        _curCoroutine = StartCoroutine(AttackTargetEffectCoroutine());
    }

    private IEnumerator AttackTargetEffectCoroutine()
    {
        Unit targetUnit = null;
        while (true)
        {
            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            
            var newTarget = FieldSystem.unitSystem.GetUnit(target);
            if (newTarget is null or Player)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, newTarget.hexPosition) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            if (FieldSystem.tileSystem.RayThroughCheck(_player.hexPosition, newTarget.hexPosition) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            if (Hex.Distance(_player.hexPosition, newTarget.hexPosition) > _player.weapon.GetRange()
                && _player.weapon is Shotgun)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            targetUnit = newTarget;
            
            aimEffectRectTsf.gameObject.SetActive(true);
            var hitRate = _player.weapon.GetFinalHitRate(targetUnit);
            float offset = 0;
            if (_player.GetSelectedAction() is FanningAction f)
            {
                offset += f.GetHitRateModifier();
            }
            else
            {
                if (_player.weapon.magazine.GetNextBullet().isGoldenBullet)
                {
                    foreach (var passive in _player.GetAllPassiveList())
                    {
                        if (passive.index == 21006)
                        {
                            offset += 20;
                            break;
                        }
                    }
                }
            }
            hitRate += offset;
            
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(Hex.Hex2World(targetUnit.hexPosition) + 
                                                                        Vector3.up * (targetUnit.transform.localScale.y * 0.5f));
            var sizeDelta = combatCanvas.sizeDelta;
            
            Vector2 worldObjectScreenPosition = new Vector2(
                ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
                ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
            aimEffectRectTsf.anchoredPosition = worldObjectScreenPosition;
            
            float size = hitRate * 0.01f;
            aimEffect.localScale = new Vector3(size, size, 1);
            
            //hitrate text
            aimEffectRectTsf.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                $"{hitRate}%";
            

            // aimEffectRectTsf.gameObject.SetActive(false);
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region DYNAMITE
    private void DynamiteTileEffect()
    {
        int range = _player.GetAction<DynamiteAction>().GetThrowingRange();
        
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, range).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));

        foreach (var tile in tiles)
        {
            if(Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            
            var go =Instantiate(attackTileEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            _effectsBase.Add(tile.hexPosition, go);
        }

        _curCoroutine = StartCoroutine(DynamiteTargetEffectCoroutine());
    }

    private IEnumerator DynamiteTargetEffectCoroutine()
    {
        int expRange = _player.GetAction<DynamiteAction>().GetExplosionRange();
        int thrRange = _player.GetAction<DynamiteAction>().GetThrowingRange();
        
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            if (Hex.Distance(target, _player.hexPosition) > thrRange)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            var tiles = FieldSystem.tileSystem.GetTilesInRange(target, expRange);
            foreach (var pos in tiles.Select(tile => tile.hexPosition)) 
            {
                if(Hex.Distance(_player.hexPosition, pos) > _player.stat.sightRange) continue;
                SetEffectTarget(pos, TileEffectType.Friendly);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region USING ITEM
    
    private void ItemTileEffect()
    {
        int range = _player.GetAction<ItemUsingAction>().GetItem().GetData().itemRange;
        
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, range).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));

        foreach (var tile in tiles)
        {
            if(Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            var go =Instantiate(attackTileEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            _effectsBase.Add(tile.hexPosition, go);
        }

        _curCoroutine = StartCoroutine(ItemTargetEffectCoroutine());
    }

    private IEnumerator ItemTargetEffectCoroutine()
    {
        int expRange = 0;
        int thrRange = _player.GetAction<ItemUsingAction>().GetItem().GetData().itemRange;
        
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            if (Hex.Distance(target, _player.hexPosition) > thrRange)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            var tiles = FieldSystem.tileSystem.GetTilesInRange(target, expRange);
            foreach (var pos in tiles.Select(tile => tile.hexPosition)) 
            {
                if(Hex.Distance(_player.hexPosition, pos) > _player.stat.sightRange) continue;
                SetEffectTarget(pos, TileEffectType.Friendly);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region COVER

    [SerializeField] private int coverEffectRange;
    private void CoverEffect()
    {
        const int range = 1;

        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, range).Where(
            tile => tile.GetTileObject<CoverableObj>() is not null);

        foreach (Tile tile in tiles)
        {
            // SetEffectBase(tile.hexPosition, TileEffectType.Normal);
        }
        
        _curCoroutine = StartCoroutine(CoverEffectCoroutine());
    }
    
    private IEnumerator CoverEffectCoroutine()
    {
        const int range = 2;
        const float diff = 1;

        while (true)
        {
            
            // 1. Set Mouse Overed Tile : Normal
            yield return null;
            ClearEffect(_effectsRelatedTarget);
            if (Player.TryGetMouseOverTilePos(out Vector3Int target) is false)
            {
                Debug.LogWarning("Tile is null");
                continue;
            }
            // if (Hex.Distance(target, _player.hexPosition) > range)
            // {
            //     Debug.LogWarning("Tile is null");
            //     continue;
            // }
            
            Tile tile = FieldSystem.tileSystem.GetTile(target);
            SetEffectTarget(target, coverPosition);
            
            if (tile.GetTileObject<CoverableObj>() is null)
            {
                Debug.LogError("target tile has no cover");
                continue;
            }
            
            
            Vector2 coverDir = 
                Hex.Hex2Orth(tile.hexPosition)
                - Hex.Hex2Orth(_player.hexPosition);
            
            var coverTiles = FieldSystem.tileSystem.GetTilesInRange(target, coverEffectRange);
            foreach (Vector3Int pos in coverTiles.Select(t => t.hexPosition))
            {
                // if (Hex.Distance(_player.hexPosition, pos) > _player.stat.sightRange) continue;
                
                Vector2 posDir = 
                    Hex.Hex2Orth(pos)
                    - Hex.Hex2Orth(target);

                float angle = Vector3.SignedAngle(coverDir, posDir,Vector3.up);
                if((angle is >= 0 - diff and <= 60 + diff) || 
                   (angle is <= 360 + diff and >= 300 - diff))
                    SetEffectTarget(pos, coverAvailable);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    private new void Awake()
    {
        base.Awake();

        _effectsBase = new Dictionary<Vector3Int, GameObject>();
        _effectsRelatedTarget = new Dictionary<Vector3Int, GameObject>();
    }

    private void ClearEffect()
    {
        // <Legacy>
        // var tiles = MainSystem.instance.tileSystem.GetAllTiles();
        // foreach (var tile in tiles) tile.effect = null;
        if(_curCoroutine is not null) StopCoroutine(_curCoroutine);
        
        ClearEffect(_effectsBase);
        ClearEffect(_effectsRelatedTarget);

        aimEffectRectTsf.gameObject.SetActive(false);
    }

    private void SetEffectBase(Vector3Int position, TileEffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.02f;
        
        var gObject = Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
        _effectsBase.Add(position, gObject);
    }
    
    private void SetEffectBase(Vector3Int position, GameObject effect)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.02f;
        
        var gObject = Instantiate(effect, worldPosition, Quaternion.identity);
        _effectsBase.Add(position, gObject);
    }
    
    private void SetEffectTarget(Vector3Int position, TileEffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.03f;

        if (_effectsRelatedTarget.ContainsKey(position) is false)
        {
            GameObject gObject = 
                Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
            _effectsRelatedTarget.Add(position, gObject);
        }
    }
    
    private void SetEffectTarget(Vector3Int position, GameObject effect)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.03f;

        if (_effectsRelatedTarget.ContainsKey(position) is false)
        {
            GameObject gObject = 
                Instantiate(effect, worldPosition, Quaternion.identity);
            _effectsRelatedTarget.Add(position, gObject);
        }
    }
    
    private GameObject GetEffect(TileEffectType type)
    {
        return type switch
        {
            TileEffectType.Friendly => friendlyEffect,
            TileEffectType.Hostile => hostileEffect,
            TileEffectType.Normal => normalEffect,
            TileEffectType.Impossible => impossibleEffect,
            TileEffectType.Invisible => invisibleEffect,
            TileEffectType.Ammunition => ammunitionEffect,
            TileEffectType.Saloon => salonEffect,
            TileEffectType.Sheriff => sheriffEffect,
            _ => null
        };
    }
    
    private TileEffectType GetEffectType(GameObject effect)
    {
        if (effect == friendlyEffect) return TileEffectType.Friendly;
        if (effect == hostileEffect) return TileEffectType.Hostile;
        if (effect == impossibleEffect) return TileEffectType.Impossible;
        if (effect == invisibleEffect) return TileEffectType.Invisible;
        if (effect == ammunitionEffect) return TileEffectType.Ammunition;
        if (effect == salonEffect) return TileEffectType.Saloon;
        if (effect == sheriffEffect) return TileEffectType.Sheriff;
        if (effect == normalEffect) return TileEffectType.Normal;
        return TileEffectType.None;
    }
    
    private static void ClearEffect(Dictionary<Vector3Int, GameObject> pool)
    {
        foreach (GameObject go in pool.Values)
        {
            Destroy(go);
        }
        
        pool.Clear();
    }
    #endregion
}
