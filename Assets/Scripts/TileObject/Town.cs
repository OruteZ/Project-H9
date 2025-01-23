using UnityEngine;
using System.Collections.Generic;

public class Town : TileObject
{
    public enum BuildingType 
    {
        NULL,
        Ammunition,
        Saloon,
        Sheriff
    }
    [SerializeField] private int _townIndex;
    [SerializeField] private BuildingType _buildingType;

    public int[] townItemIndexes;
    
    protected override void SetTile(Tile t)
    {
        base.SetTile(t);

        t.walkable = true;
        t.visible = true;
        t.rayThroughable = false;

        UIManager.instance.gameSystemUI.townUI.AddTownIcon(hexPosition, _buildingType);
    }

    public override void OnHexCollisionEnter(Unit other)
    {
        //Debug.Log($"플레이어 진입 : {_townIndex}번 마을의 {_buildingType} 건물");
        PlayerEvents.OnPlayerEnterTown.Invoke(hexPosition, _townIndex, _buildingType);
    }


    public override string[] GetArgs()
    {
        return new[] { _townIndex.ToString() };
    }

    public override void SetArgs(string[] args)
    {
        if(args.Length != 1) throw new System.Exception();
        
        _townIndex = int.Parse(args[0]);
    }

    public TileEffectType GetTileEffectType() 
    {
        Dictionary<BuildingType, TileEffectType> effect = new Dictionary<BuildingType, TileEffectType>
        {
            {BuildingType.NULL,         TileEffectType.Normal },
            {BuildingType.Ammunition,   TileEffectType.Ammunition },
            {BuildingType.Saloon,        TileEffectType.Saloon },
            {BuildingType.Sheriff,      TileEffectType.Sheriff },
        };
        effect.TryGetValue(_buildingType, out TileEffectType effType);
        return effType;
    }

    public int GetTownIndex() => _townIndex;
    public BuildingType GetTownType() => _buildingType;
}
