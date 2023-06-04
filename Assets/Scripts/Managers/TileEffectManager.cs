using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;

public enum EffectType
{
    None = 0,
    Normal = 1,
    Friendly = 2,
    Hostile = 3,
    Impossible = 4,
    Invisible = 5,
    FogOfWar = 6,
}

/// <summary>
/// 타일을 넘겨받으면 원하는 이펙트를 타일에 적용시켜주는 클래스
/// </summary>
public class TileEffectManager : Singleton<TileEffectManager>
{
    [SerializeField] private Material _friendlyEffect;
    [SerializeField] private Material _hostileEffect;
    [SerializeField] private Material _normalEffect;
    [SerializeField] private Material _impossibleEffect;
    [SerializeField] private Material _invisibleEffect;
    [SerializeField] private Material _fogOfWarEffect;

    /// <summary>
    /// 모든 이펙트를 초기화 합니다.
    /// </summary>
    public static void ClearEffect()
    {
        var tiles = MainSystem.instance.tileSystem.GetAllTiles();
        foreach (var tile in tiles) tile.effect = null;
    }

    /// <summary>
    /// 타일에 이펙트를 적용합니다.
    /// </summary>
    public static void SetEffect(IEnumerable<Tile> tiles, EffectType type)
    {
        var effect = instance.GetEffect(type);
        foreach (var tile in tiles)
        {
            tile.effect = effect;
        }
    }

    public static void SetEffect(Tile tile, EffectType type)
    {
        tile.effect = instance.GetEffect(type);
    }
    
    private Material GetEffect(EffectType type)
    {
        return type switch
        {
            EffectType.Friendly => _friendlyEffect,
            EffectType.Hostile => _hostileEffect,
            EffectType.Normal => _normalEffect,
            EffectType.Impossible => _impossibleEffect,
            EffectType.Invisible => _invisibleEffect,
            EffectType.FogOfWar => _fogOfWarEffect,
            _ => null
        };
    }

    private EffectType GetEffectType(Material effect)
    {
        if (effect == _friendlyEffect) return EffectType.Friendly;
        if (effect == _hostileEffect) return EffectType.Hostile;
        if (effect == _impossibleEffect) return EffectType.Impossible;
        if (effect == _invisibleEffect) return EffectType.Invisible;
        if (effect == _fogOfWarEffect) return EffectType.FogOfWar;
        if (effect == _normalEffect) return EffectType.Normal;
        return EffectType.None;
    }
}
