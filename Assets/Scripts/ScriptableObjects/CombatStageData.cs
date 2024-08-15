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
    
    [SerializeField] private TileData[] tileData;
    [SerializeField] private TileObjectData[] tileObjectData;
    [SerializeField] private EnvironmentData[] envData;
    
    [SerializeField]
    private List<Vector3Int> enemySpawnPoints;
    
    [SerializeField]
    private Vector3Int playerSpawnPoint;
    
    [SerializeField]
    private StageStyle stageStyle;
    
    public List<TileData> GetTileDataList()
    {
        //return deep copy of data list
        return tileData.ToList();
    }
    
    public List<TileObjectData> GetTileObjectDataList()
    {
        //return copy of data list
        return tileObjectData.ToList();
    }
    
    public List<EnvironmentData> GetEnvDataList()
    {
        //return copy of data list
        return envData.ToList();
    }
    
    public void SetTileDataList(List<TileData> list)
    {
        tileData = list.ToArray();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
    
    public void SetTileObjectDataList(List<TileObjectData> list)
    {
        tileObjectData = list.ToArray();
        
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        
#endif
    }
    
    public void SetEnvDataList(List<EnvironmentData> list)
    {
        envData = list.ToArray();
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }

    public bool TryGetEnemyPoints(LinkData linkData, out Vector3Int[] points)
    {
        enemySpawnPoints ??= new List<Vector3Int>();
        
        if(enemySpawnPoints.Count < linkData.combatEnemy.Length)
        {
            Debug.LogError(
                "Not enough enemy spawn points. " +
                $"spawn point count : {enemySpawnPoints.Count} / enemy count : {linkData.combatEnemy.Length}," +
                $"enemy idx info : {linkData.index}," +
                $"map idx info : {name}"
            );
            points = Array.Empty<Vector3Int>();
            return false;
        }
        
        // set random points
        points = 
            enemySpawnPoints.
            OrderBy(x => UnityEngine.Random.value).
            Take(linkData.combatEnemy.Length).
            ToArray();
        
        return true;
    }
    
    public bool TryGetAllEnemyPoints(out Vector3Int[] points)
    {
        enemySpawnPoints ??= new List<Vector3Int>();
        
        // set random points
        points = 
            enemySpawnPoints.
                OrderBy(x => UnityEngine.Random.value).
                ToArray();
        
        return true;
    }
    
    public bool TryGetPlayerPoint(out Vector3Int point)
    {
        point = playerSpawnPoint;
        return true;
    }
    
    public void SetEnemyPoints(IEnumerable<Vector3Int> points)
    {
        enemySpawnPoints = points.ToList();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
    
    public void SetPlayerPoint(Vector3Int point)
    {
        playerSpawnPoint = point;
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
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
    public GameObject prefab;
    public Vector3Int hexPosition;
    public float rotation;
    public Vector3 positionOffset;
    public Vector3 scale;
    
    public string[] arguments;

    public TileObjectData(TileObject obj)
    {
        hexPosition = obj.hexPosition;
        positionOffset = obj.gameObject.transform.position - Hex.Hex2World(hexPosition);
        rotation = obj.gameObject.transform.localRotation.y;
        scale = obj.gameObject.transform.localScale;
        arguments = obj.GetArgs();
        
        #if UNITY_EDITOR
        if(obj.gameObject.GetPrefabDefinition() is GameObject p) prefab = p;
        else
        {
            Debug.LogError("TileObjectData : Prefab is null");
            prefab = null;
        }
        #else
        prefab = null;
        #endif
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