using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileSystem : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject fogOfWarPrefab;
    public Transform fogs;
    
    private Dictionary<Vector3Int, Tile> _tiles;

    public GameObject map;
    private HexGridLayout _gridLayout;

    private HexGridLayout gridLayout => _gridLayout ??= map.GetComponent<HexGridLayout>();

    private void Awake()
    {
        _tiles = new Dictionary<Vector3Int, Tile>();
        _gridLayout = map.GetComponent<HexGridLayout>();
    }

    public List<Tile> GetAllTiles()
    {
        var result = new List<Tile>();
        result.AddRange(_tiles.Values);

        return result;
    }

    public List<Vector3Int> GetAllTilePos()
    {
        var result = new List<Vector3Int>();
        result.AddRange(_tiles.Values.Select(x => x.hexPosition));

        return result;
    }

    public void SetUpTilesAndObjects()
    {
        var tilesInChildren = GetComponentsInChildren<Tile>();  
        foreach (Tile t in tilesInChildren)
        {
            AddTile(t);
            if (GameManager.instance.CompareState(GameState.World))
            {
                var fow = Instantiate(fogOfWarPrefab, fogs).GetComponent<FogOfWar>(); 
                fow.hexPosition = t.hexPosition;
            }
            
        }

        var objects = GetComponentsInChildren<TileObject>().ToList();
        foreach (TileObject obj in objects)
        {
            obj.SetUp();
        }

        _gridLayout.LayoutGrid();
    }

    /// <summary>
    /// 지정된 좌표에 타일을 생성합니다. walkable, visible, rayThroughable속성을 설정할 수 있습니다.
    /// </summary>
    /// <param name="position">타일이 생성될 hex좌표입니다.</param>
    /// <param name="walkable">타일의 walkable(이동가능) 속성입니다.</param>
    /// <param name="visible">타일의 visible(시야가 보임) 속성입니다.</param>
    /// <param name="rayThroughable">타일의 rayThroughable(ray가 통과 가능함) 속성입니다.</param>
    /// <returns> 추가된 Tile을 반환합니다. </returns>
    private Tile AddTile(Vector3Int position, bool walkable = true, bool visible = true, bool rayThroughable = true)
    {
        var tile = Instantiate(tilePrefab, transform).GetComponent<Tile>();
        tile.hexPosition = position;
        tile.walkable = walkable;
        tile.visible = visible;
        tile.rayThroughable = rayThroughable;
        if (!_tiles.TryAdd(position, tile))
        {
            throw new Exception("Tile 추가에 실패했습니다.");
        }

        return tile;
    }
    
    /// <summary>
    /// 지정된 좌표에 타일을 생성합니다. walkable, visible, rayThroughable속성을 설정할 수 있습니다.
    /// </summary>
    /// <param name="tile">타일 class입니다.</param>
    /// <returns> 추가된 Tile을 반환합니다. </returns>
    private Tile AddTile(Tile tile)
    {
        if (!_tiles.TryAdd(tile.hexPosition, tile))
        {
            throw new Exception("Tile 추가에 실패했습니다.");
        }
        
        return tile;
    }

    /// <summary>
    /// 해당 Hex좌표에 해당하는 Tile을 가져옵니다.
    /// </summary>
    /// <param name="position">Hex좌표</param>
    /// <returns>Tile</returns>
    public Tile GetTile(Vector3Int position)
    {
        return _tiles.TryGetValue(position, out var tile) ? tile : null;
    }

    /// <summary>
    /// start지점에서 limitMovement 칸 이내에 도달 할 수 있는 모든 Tile을 반환합니다.
    /// </summary>
    /// <param name="start">이동 시작점</param>
    /// <param name="maxLength">최대 이동 칸 수</param>
    /// <returns>도달 가능한 모든 Tile들이 담긴 List</returns>
    public IEnumerable<Tile> GetWalkableTiles(Vector3Int start, int maxLength)
    {
        var visited = new HashSet<Vector3Int> { start };
        var result = new List<Tile> { GetTile(start) };
        var container = new Queue<Vector3Int>();
        container.Enqueue(start);

        for(int cnt = 0; cnt < maxLength; cnt++)
        {
            int length = container.Count;
            for(int i = 0; i < length; i++)
            {
                if (!container.TryDequeue(out var current)) return result;

                foreach (var dir in Hex.directions)
                {
                    var next = current + dir;
                    if (visited.Contains(next)) continue;
                    
                    var tile = GetTile(next);
                    if (tile is null) continue;
                    if (!tile.walkable) continue;
                    
                    result.Add(GetTile(next));
                    container.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return result;
    }

    public IEnumerable<Tile> GetTilesInRange(Vector3Int start, int range_)
    {
        var list = Hex.GetCircleGridList(range_, start);
        var ret = new List<Tile>();

        foreach (var t in list)
        {
            var tile = GetTile(t);
            if (tile is not null) ret.Add(tile);
        }

        return ret;
    }

    /// <summary>
    /// start지점에서 destination 까지의 경로를 리스트에 저장하여 반환합니다.
    /// 시작점과 도착지점을 포함한 경로를 반환합니다.
    /// maxLength로 입력되는 최대 길이 이상의 길은 탐색할 수 없습니다.
    /// </summary>
    /// <param name="start">시작지점</param>
    /// <param name="destination">도착지점</param>
    /// <param name="maxLength">최대 길이, 기본값은 100</param>
    /// <returns>경로를 담은 리스트</returns>
    public List<Tile> FindPath(Vector3Int start, Vector3Int destination, int maxLength = 100)
    {
        var visited = new HashSet<Vector3Int> { start };
        var container = new Queue<PathNode>();
        container.Enqueue(new PathNode(start));

        for(int cnt = 0; cnt < maxLength + 1; cnt++)
        {
            int length = container.Count;
            for(int i = 0; i < length; i++)
            {
                if (!container.TryDequeue(out var current)) return null;
                if (current.position == destination)
                {
                    var result = new List<Tile>();
                    while (current.from is not null)
                    {
                        result.Add(GetTile(current.position));
                        current = current.from;
                    }
                    result.Add(GetTile(start));
                    result.Reverse();
                    return result;
                }

                foreach (var dir in Hex.directions)
                {
                    var next = current.position + dir;
                    if (visited.Any(n => n == next)) continue;
                    
                    var tile = GetTile(next);
                    if (tile is null) continue;
                    if (tile.walkable is false) continue;
                    
                    container.Enqueue(new PathNode(next, from:current));
                    visited.Add(next);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// start 지점에서 target까지 Ray를 발사합니다. 가로막히는 벽의 기준은 ray-throughable 변수입니다.
    /// </summary>
    /// <param name="start">시작점의 좌표</param>
    /// <param name="target">목적지의 좌표</param>
    /// <returns>두 지점 사이 장애물이 없으면 true를 반환합니다. </returns>
    public bool RayCast(Vector3Int start, Vector3Int target)
    { 
        var line1 = Hex.LineDraw (start, target);
        var line2 = Hex.LineDraw_(start, target);

        bool result = true;

        for (int i = 0; i < line1.Count; i++)
        {
            var ret1 = GetTile(line1[i]);
            var ret2 = GetTile(line2[i]);
            if (ret1 is null || ret2 is null) continue;
            if (ret1.rayThroughable || ret2.rayThroughable) continue;

            return false;
        }

        return true;
    }
    
    /// <summary>
    /// start 지점에서 target까지 Ray를 발사합니다. 가로막히는 타일의 기준은 visible 변수입니다.
    /// </summary>
    /// <param name="start">시작점의 좌표</param>
    /// <param name="target">목적지의 좌표</param>
    /// <returns>두 지점 사이 장애물이 없으면 true를 반환합니다. </returns>
    public bool VisionCast(Vector3Int start, Vector3Int target)
    { 
        var line1 = Hex.LineDraw(start, target);
        var line2 = Hex.LineDraw_(start, target);

        for (int i = 0; i < line1.Count - 1; i++)
        {
            var ret1 = GetTile(line1[i]);
            var ret2 = GetTile(line2[i]);
            if (ret1 is null || ret2 is null) continue;
            if (ret1.visible || ret2.visible) continue;
            return false;
        }

        return true;
    }
    
    //demo code : 간단한 맵 생성용
    [Header("Hex World Inspector")]
    public int range;

    [Header("Square World Inspector")] 
    public int width;
    public int height;
    
    [ContextMenu("Create Hex World")]
    public void CreateHexWorld()
    {
        var positions = Hex.GetCircleGridList(range, Hex.zero);
        Debug.Log(positions.Count);
        foreach (var pos in positions)
        {
            var tile = Instantiate(tilePrefab, map.transform).GetComponent<Tile>();
            tile.hexPosition = pos;
            tile.visible = tile.walkable = tile.rayThroughable = true;
            // if(pos == Vector3Int.zero || pos == new Vector3Int(0, -1, 1))
            //     isWall = false;
            //
            // isWall = false;
            // var tile = AddTile(pos, walkable : !isWall, visible : !isWall, rayThroughable: !isWall);
            //
            // TileEffectManager.SetEffect(tile, isWall ? EffectType.Impossible : EffectType.Normal);
        }
        
        gridLayout.LayoutGrid();
    }

    [ContextMenu("Create Rect World")]
    public void CreateRectWorld()
    {
        var positions = Hex.GetSquareGridList(width, height);
        Debug.Log(positions.Count);
        foreach (var pos in positions)
        {
            var tile = Instantiate(tilePrefab, map.transform).GetComponent<Tile>();
            tile.hexPosition = pos;
            tile.visible = tile.walkable = tile.rayThroughable = true;
            // if(pos == Vector3Int.zero || pos == new Vector3Int(0, -1, 1))
            //     isWall = false;
            //
            // isWall = false;
            // var tile = AddTile(pos, walkable : !isWall, visible : !isWall, rayThroughable: !isWall);
            //
            // TileEffectManager.SetEffect(tile, isWall ? EffectType.Impossible : EffectType.Normal);
        }

        gridLayout.LayoutGrid();
    }
    //
    // [ContextMenu("Create World no wall")]
    // private void CreateDemoWorld2()
    // {
    //     var positions = Hex.GetGridsWithRange(range, Hex.zero);
    //     foreach (var pos in positions)
    //     {
    //         AddTile(pos);
    //     }
    // }

    [ContextMenu("Remove Demo World")]
    private void RemoveDemoWorld()
    {
        var tiles = GetComponentsInChildren<Tile>();
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }

        gridLayout.ClearGrid();
    }

    // [Header("World to combat Test")] 
    // public Vector3Int playerPosition;
    // public Scene combatScene;
    //
    // [ContextMenu("World to Combat")]
    // private void ChangeToCombatScene()
    // {
    //     DontDestroyOnLoad(gameObject);
    //     SceneManager.LoadScene("CombatScene");
    //     gameObject.SetActive(false);
    // }
    //
    // [ContextMenu("Combat to World")]
    // private void CombatSceneToThis()
    // {
    //     SceneManager.LoadScene("WorldScene");
    //     gameObject.SetActive(true);
    // }
    
    //demo end
}

/// <summary>
/// Path finding에 사용되는 Node 클래스
/// </summary>
internal class PathNode
{
    public PathNode(Vector3Int position,int g=0, int h=0, PathNode from=null)
    {
        this.position = position;
        this.from = from;
    }
    
    public int G, H;
    public int F => G + H;

    public Vector3Int position;
    public readonly PathNode from;
}