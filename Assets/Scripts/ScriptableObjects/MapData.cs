using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
[CreateAssetMenu(fileName = "SaveFile", menuName = "ScriptableObjects/Map Save File", order = 1)]
public class MapData : ScriptableObject
{
    public TileData[] tileData;
    public ObjectMapData[] objectData;
    public EnvironmentData[] envData;
}

/// <summary>
/// 타일의 정보를 담은 타일 데이터
/// </summary>
[System.Serializable]
public struct TileData
{
    public Vector3 hexPosition;

    public bool visible;
    public bool walkable;
    public bool rayThroughable;
    public bool gridVisible;

    public TileData(Vector3 pos, bool visible, bool walkable, bool rayThroughable, bool gridVisible)
    {
        hexPosition = pos;

        this.visible = visible;
        this.walkable = walkable;
        this.rayThroughable = rayThroughable;
        this.gridVisible = gridVisible;
    }

    public TileData(Tile tile)
    {
        hexPosition = tile.hexPosition;
        visible = tile.visible;
        rayThroughable = tile.rayThroughable;
        gridVisible = tile.gridVisible;
        walkable = tile.walkable;
    }

    public static TileData GetWaterTile()
    {
        return new TileData(Hex.zero, true, false, true, false);
    }

    public static TileData GetWallTile()
    {
        return new TileData(Hex.zero, false, false, false, true);
    }

    public static TileData GetTreeTile()
    {
        return new TileData(Hex.zero, true, false, false, true);
    }
}

/// <summary>
/// 오브젝트의 정보를 담은 데이터
/// </summary>
[System.Serializable]
public struct ObjectMapData
{
    public int id;
    public Vector3 hexPosition;
    public float rotation;
    public string[] arguments;

    public ObjectMapData(TileObject obj)
    {
        id = obj.id;
        hexPosition = obj.hexPosition;
        rotation = obj.gameObject.transform.localRotation.y;
        arguments = obj.GetArgs();
    }
}

/// <summary>
/// 게임 플레이와 무관한 환경 / 배치용 데이터
/// </summary>
[System.Serializable]
public struct EnvironmentMapData
{
    public GameObject prefab;
    public Vector3 hexPosition;
    public int rotation;
}