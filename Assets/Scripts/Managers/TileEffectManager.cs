using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Generic;
using UnityEngine.Rendering;

public enum EffectType
{
    None = 0,
    Normal = 1,
    Friendly = 2,
    Hostile = 3,
    Impossible = 4,
    Invisible = 5,
}

/// <summary>
/// 타일을 넘겨받으면 원하는 이펙트를 타일에 적용시켜주는 클래스
/// </summary>
public class TileEffectManager : Singleton<TileEffectManager>
{
    public GameObject friendlyEffect;
    public GameObject hostileEffect;
    public GameObject normalEffect;
    public GameObject impossibleEffect;
    public GameObject invisibleEffect;

    private Player _player;
    private Stack<GameObject> _effectStackBase;
    private Stack<GameObject> _effectStackRelatedTarget;

    private Coroutine _curCoroutine;

    public Material combatFowMaterial;

    [field: Header("Attack Effect")] 
    public GameObject attackTileEffect;
    public GameObject attackOutOfRangeEffect;
    public GameObject attackUnitEffect;
    public RectTransform aimEffectRectTsf; 
    public RectTransform combatCanvas;
    public RectTransform aimEffect;
    
    
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
                break;
            case ActionType.Idle:
                break;
            case ActionType.Reload:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MovableTileEffect()
    {
        int range = _player.currentActionPoint;
        Vector3Int start = _player.hexPosition;

        var tiles = FieldSystem.tileSystem.GetWalkableTiles(start, range);
        foreach (var tile in tiles)
        {
            if (GameManager.instance.CompareState(GameState.Combat))
            {
                if (Hex.Distance(tile.hexPosition, _player.hexPosition) > _player.GetStat().sightRange) continue;
                if (!FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition)) continue;
            }
            else //GameState.World
            {
                bool containsFog = tile.interactiveObjects.OfType<FogOfWar>().Any();
                if (containsFog) continue;
            }
            SetEffectBase(tile.hexPosition, EffectType.Normal);
        }

        _curCoroutine = StartCoroutine(MovableTileEffectCoroutine());
    }

    private IEnumerator MovableTileEffectCoroutine()
    {
        while (true)
        {
            while (_effectStackRelatedTarget.TryPop(out var effect)) { Destroy(effect); }

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                yield return null;
                continue;
            }
            
            var route = FieldSystem.tileSystem.FindPath(_player.hexPosition, target);
            if (route is null)
            {
                yield return null;
                continue;
            }
            
            if(route.Count - 1 <= _player.currentActionPoint) foreach (var tile in route)
            {
                var pos = tile.hexPosition;
                SetEffectTarget(pos, EffectType.Friendly);
            }
    
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void AttackTileEffect()
    {
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, _player.weapon.GetRange()).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));
        var units = FieldSystem.unitSystem.units.Where(
            unit => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) && unit is not Player);

        foreach (var tile in tiles)
        {
            if (FieldSystem.unitSystem.GetUnit(tile.hexPosition) is not null) continue;

            var go =Instantiate(attackTileEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            _effectStackBase.Push(go);
        }

        foreach (var unit in units)
        {
            if (FieldSystem.tileSystem.RayThroughCheck(_player.hexPosition, unit.hexPosition) is false) continue;
            
            var go = Instantiate((_player.weapon.GetRange() >= Hex.Distance(_player.hexPosition, unit.hexPosition) ? 
                attackUnitEffect : attackOutOfRangeEffect), Hex.Hex2World(unit.hexPosition), Quaternion.identity);
            _effectStackBase.Push((GameObject)go);
        }

        _curCoroutine = StartCoroutine(AttackTargetEffectCoroutine());
    }

    private IEnumerator AttackTargetEffectCoroutine()
    {
        while (true)
        {
            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            Unit targetUnit = FieldSystem.unitSystem.GetUnit(target);
            if (targetUnit is null)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            aimEffectRectTsf.gameObject.SetActive(true);
            
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(Hex.Hex2World(targetUnit.hexPosition) + 
                                                                        Vector3.up * (targetUnit.transform.localScale.y * 0.5f));
            var sizeDelta = combatCanvas.sizeDelta;
            
            Vector2 worldObjectScreenPosition = new Vector2(
                ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
                ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
            aimEffectRectTsf.anchoredPosition = worldObjectScreenPosition;
            aimEffect.localScale = Vector3.one * _player.weapon.GetFinalHitRate(targetUnit);

            // aimEffectRectTsf.gameObject.SetActive(false);
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    private new void Awake()
    {
        base.Awake();

        _effectStackBase = new Stack<GameObject>();
        _effectStackRelatedTarget = new Stack<GameObject>();
    }

    /// <summary>
    /// 모든 이펙트를 초기화 합니다.
    /// </summary>
    private void ClearEffect()
    {
        // <Legacy>
        // var tiles = MainSystem.instance.tileSystem.GetAllTiles();
        // foreach (var tile in tiles) tile.effect = null;
        if(_curCoroutine is not null) StopCoroutine(_curCoroutine);

        while (_effectStackBase.TryPop(out var effect))
        {
            Destroy(effect);
        }

        while (_effectStackRelatedTarget.TryPop(out var effect))
        {
            Destroy(effect);
        }
    }

    private void SetEffectBase(Vector3Int position, EffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.02f;
        
        var gObject = Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
        _effectStackBase.Push(gObject);
    }
    
    private void SetEffectTarget(Vector3Int position, EffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.03f;

        var gObject = Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
        _effectStackRelatedTarget.Push(gObject);
    }
    
    private GameObject GetEffect(EffectType type)
    {
        return type switch
        {
            EffectType.Friendly => friendlyEffect,
            EffectType.Hostile => hostileEffect,
            EffectType.Normal => normalEffect,
            EffectType.Impossible => impossibleEffect,
            EffectType.Invisible => invisibleEffect,
            _ => null
        };
    }

    private EffectType GetEffectType(GameObject effect)
    {
        if (effect == friendlyEffect) return EffectType.Friendly;
        if (effect == hostileEffect) return EffectType.Hostile;
        if (effect == impossibleEffect) return EffectType.Impossible;
        if (effect == invisibleEffect) return EffectType.Invisible;
        if (effect == normalEffect) return EffectType.Normal;
        return EffectType.None;
    }
}
