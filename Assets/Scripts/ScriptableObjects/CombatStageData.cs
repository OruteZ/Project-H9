using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
[CreateAssetMenu(fileName = "SaveFile", menuName = "ScriptableObjects/Map Save File", order = 1)]
public class CombatStageData : ScriptableObject
{
    public static int GetLinkCount()
    {
        return FindObjectOfType<LinkDatabase>().Length();
    }
    
    public TileData[] tileData;
    public TileObjectData[] tileObjectData;
    public EnvironmentData[] envData;
    
    [SerializeField]
    private List<PositionList> enemySpawnPoints;
    
    [SerializeField]
    private List<Vector3Int> playerSpawnPoint;

    public bool TryGetEnemyPoints(int linkIndex, out Vector3Int[] points)
    {
        enemySpawnPoints ??= new List<PositionList>();
        if(enemySpawnPoints.Count <= linkIndex)
        {
            points = Array.Empty<Vector3Int>();
            return false;
        }
        
        points = enemySpawnPoints[linkIndex].list.ToArray();
        return true;
    }
    
    public bool TryGetPlayerPoint(int linkIndex, out Vector3Int point)
    {
        if(playerSpawnPoint.Count <= linkIndex)
        {
            point = Hex.zero;
            return false;
        }
        
        point = playerSpawnPoint[linkIndex];
        return true;
    }
    
    public void SetEnemyPoints(int linkIndex, Vector3Int[] points)
    {
        if(enemySpawnPoints.Count <= linkIndex)
        {
            var diff = linkIndex - enemySpawnPoints.Count;
            for (int i = 0; i <= diff; i++)
            {
                enemySpawnPoints.Add(new PositionList());
            }
        }
        
        enemySpawnPoints[linkIndex].list = points.ToList();
        EditorUtility.SetDirty(this);
    }
    
    public void SetPlayerPoint(int linkIndex, Vector3Int point)
    {
        if(playerSpawnPoint.Count <= linkIndex)
        {
            var diff = linkIndex - playerSpawnPoint.Count;
            for (int i = 0; i <= diff; i++)
            {
                playerSpawnPoint.Add(Hex.zero);
            }
        }

        playerSpawnPoint[linkIndex] = point;
        EditorUtility.SetDirty(this);
    }
}

/// <summary>
/// 타일의 정보를 담은 타일 데이터
/// </summary>
[System.Serializable]
public struct TileData
{
    public Vector3Int hexPosition;

    public bool visible;
    public bool walkable;
    public bool rayThroughable;
    public bool gridVisible;

    public TileData(Vector3Int pos, bool visible, bool walkable, bool rayThroughable, bool gridVisible)
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
public struct TileObjectData
{
    public Vector3Int hexPosition;
    public float rotation;
    public string[] arguments;

    public TileObjectData(TileObject obj)
    {
        hexPosition = obj.hexPosition;
        rotation = obj.gameObject.transform.localRotation.y;
        arguments = obj.GetArgs();
    }
}

/// <summary>
/// 게임 플레이와 무관한 환경 / 배치용 데이터
/// </summary>
[System.Serializable]
public struct EnvironmentData
{
    public Material material;
    public Mesh mesh;
    public bool hexPositioned;
    
    public Vector3Int hexPosition;
    public float height;
    
    public Vector3 position;
    public float rotation;
    public Vector3 scale;
    
    public EnvironmentData(GameObject env)
    {
        var meshFilter = env.GetComponent<MeshFilter>();
        var meshRenderer = env.GetComponent<MeshRenderer>();
        
        material = meshRenderer.sharedMaterial;
        mesh = meshFilter.sharedMesh;
        position = env.transform.position;
        
        //try get hexPosition
        if (env.TryGetComponent(out HexTransform hexTransform))
        {
            hexPosition = hexTransform.position;
            hexPositioned = true;
        }
        else
        {
            hexPosition = Hex.zero;
            hexPositioned = false;
        }
        
        rotation = env.transform.localRotation.eulerAngles.y;
        scale = env.transform.localScale;

        height = position.y;
    }
}

[System.Serializable]
public class PositionList
{
    public List<Vector3Int> list;
}