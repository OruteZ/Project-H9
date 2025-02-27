using UnityEngine;
using System.Collections.Generic;

public class Town : TileObject
{
    public enum BuildingType 
    {
        NULL,
        Ammunition,
        Saloon,
        Sheriff,
        Station
    }
    [SerializeField] private int _townIndex;
    [SerializeField] private BuildingType _buildingType;
    [SerializeField] private Hex.Direction _buildingDirection = Hex.Direction.UpLeft;

    Vector3Int _buildingIconHexPos = Vector3Int.zero;
    public int[] townItemIndexes;
    
    protected override void SetTile(Tile t)
    {
        base.SetTile(t);

        t.walkable = true;
        t.visible = true;
        t.rayThroughable = false;

        switch (_buildingDirection)
        {
            //31 7 -38
            case Hex.Direction.Right:
                {
                    _buildingIconHexPos = new Vector3Int(1, 0, -1);
                    break;
                }
            case Hex.Direction.DownRight:
                {
                    _buildingIconHexPos = new Vector3Int(0, 1, -1);
                    break;
                }
            case Hex.Direction.DownLeft:
                {
                    _buildingIconHexPos = new Vector3Int(-1, 1, 0);
                    break;
                }
            case Hex.Direction.Left:
                {
                    _buildingIconHexPos = new Vector3Int(-1, 0, 1);
                    break;
                }
            case Hex.Direction.UpLeft:
                {
                    _buildingIconHexPos = new Vector3Int(0, -1, 1);
                    break;
                }
            case Hex.Direction.UpRight:
                {
                    _buildingIconHexPos = new Vector3Int(1, -1, 0);
                    break;
                }
        }

        UIManager.instance.gameSystemUI.townUI.AddTownIcon(hexPosition + _buildingIconHexPos * 2, _buildingType);
    }
    

    public override void OnHexCollisionEnter(Unit other)
    {
        if (!gameObject.activeSelf) return;
        //Debug.Log($"플레이어 진입 : {_townIndex}번 마을의 {_buildingType} 건물");
        PlayerEvents.OnPlayerEnterTown.Invoke(hexPosition, _buildingIconHexPos, _townIndex, _buildingType);
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
            {BuildingType.Station,      TileEffectType.Sheriff },
        };
        effect.TryGetValue(_buildingType, out TileEffectType effType);
        return effType;
    }

    public int GetTownIndex() => _townIndex;
    public BuildingType GetTownType() => _buildingType;
}
