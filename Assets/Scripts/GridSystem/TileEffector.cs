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
public class TileEffector : Singleton<TileEffector>
{
    [SerializeField] private Material friendlyEffect;
    [SerializeField] private Material hostileEffect;
    [SerializeField] private Material normalEffect;
    [SerializeField] private Material impossibleEffect;
    [SerializeField] private Material invisibleEffect;
    [SerializeField] private Material fogOfWarEffect;

    /// <summary>
    /// 모든 이펙트를 초기화 합니다.
    /// </summary>
    public static void ClearEffect()
    {
        var tiles = CombatManager.Instance.tileSystem.GetAllTiles();
        foreach (var tile in tiles) tile.Effect = null;
    }

    /// <summary>
    /// 타일에 이펙트를 적용합니다.
    /// </summary>
    public static void SetEffect(IEnumerable<Tile> tiles, EffectType type)
    {
        var effect = Instance.GetEffect(type);
        foreach (var tile in tiles)
        {
            tile.Effect = effect;
        }
    }

    public static void SetEffect(Tile tile, EffectType type)
    {
        tile.Effect = Instance.GetEffect(type);
    }
    
    private Material GetEffect(EffectType type)
    {
        return type switch
        {
            EffectType.Friendly => friendlyEffect,
            EffectType.Hostile => hostileEffect,
            EffectType.Normal => normalEffect,
            EffectType.Impossible => impossibleEffect,
            EffectType.Invisible => invisibleEffect,
            EffectType.FogOfWar => fogOfWarEffect,
            _ => null
        };
    }

    private EffectType GetEffectType(Material effect)
    {
        if (effect == friendlyEffect) return EffectType.Friendly;
        if (effect == hostileEffect) return EffectType.Hostile;
        if (effect == impossibleEffect) return EffectType.Impossible;
        if (effect == invisibleEffect) return EffectType.Invisible;
        if (effect == fogOfWarEffect) return EffectType.FogOfWar;
        if (effect == normalEffect) return EffectType.Normal;
        return EffectType.None;
    }
}