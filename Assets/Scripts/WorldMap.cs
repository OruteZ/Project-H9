using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class WorldMap : MonoBehaviour
{
    public GameObject tileObject;
    private Dictionary<Vector3Int, Tile> _tiles;

    private Queue<Tile> _highlightQueue;

    private void Awake()
    {
        _tiles = new Dictionary<Vector3Int, Tile>();
        _highlightQueue = new Queue<Tile>();
    }

    public void AddTile(Tile tile)
    {
        _tiles.TryAdd(tile.hexTransform.position, tile);
    }

    public void AddTile(Vector3Int position, bool reachable = true, bool passable = true)
    {
        var tile = Instantiate(tileObject, transform).GetComponent<Tile>();
        tile.hexTransform.position = position;
        tile.walkable = reachable;
        tile.passable = passable;
        _tiles.TryAdd(position, tile);
    }

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
    public IEnumerable<Tile> GetReachableTiles(Vector3Int start, int maxLength)
    {
        var visited = new HashSet<Vector3Int> { start };
        var result = new List<Tile>() { GetTile(start) };
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
                    if (tile == null) continue;
                    if (!tile.walkable) continue;
                    
                    result.Add(GetTile(next));
                    container.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// start지점에서 destination 까지의 경로를 연결리스트에 저장하여 반환합니다.
    /// 시작점과 도착지점을 포함한 경로를 반환합니다.
    /// maxLength로 입력되는 최대 길이 이상의 길은 탐색할 수 없습니다.
    ///
    /// A*로 구현하고싶었으나 실패하여 BFS로 구현된 상태입니다.
    /// </summary>
    /// <param name="start">시작지점</param>
    /// <param name="destination">도착지점</param>
    /// <param name="maxLength">최대 길이, 기본값은 10</param>
    /// <returns>경로를 담은 리스트</returns>
    public IEnumerable<Tile> FindPath(Vector3Int start, Vector3Int destination, int maxLength = 100)
    {
        var visited = new HashSet<Vector3Int> { start };
        var container = new Queue<PathNode>();
        container.Enqueue(new PathNode(start));

        for(int cnt = 0; cnt < maxLength; cnt++)
        {
            int length = container.Count;
            for(int i = 0; i < length; i++)
            {
                if (!container.TryDequeue(out var current)) return null;
                if (current.pos == destination)
                {
                    var result = new List<Tile>();
                    while (current.from != null)
                    {
                        result.Add(GetTile(current.pos));
                        current = current.from;
                    }
                    result.Add(GetTile(start));
                    result.Reverse();
                    return result;
                }

                foreach (var dir in Hex.directions)
                {
                    var next = current.pos + dir;
                    if (visited.Any(n => n == next)) continue;
                    
                    var tile = GetTile(next);
                    if (tile == null) continue;
                    if (!tile.walkable) continue;
                    
                    container.Enqueue(new PathNode(next, from:current));
                    visited.Add(next);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// start 지점에서 target까지 Ray를 발사합니다.
    /// </summary>
    /// <param name="start">시작점의 좌표</param>
    /// <param name="target">목적지의 좌표</param>
    /// <param name="ret">만약 가로막는 타일이 있을 경우, ret에 해당 타일을 반환합니다.</param>
    /// <returns>두 지점 사이 장애물이 없으면 true를 반환합니다. </returns>
    public bool RayCast(Vector3Int start, Vector3Int target)
    { 
        var line1 = Hex.LineDraw(start, target);
        var line2 = Hex.LineDraw_(start, target);

        bool result = true;
        Tile ret1, ret2;
        
        for (int i = 0; i < line1.Count; i++)
        {
            ret1 = GetTile(line1[i]);
            ret2 = GetTile(line2[i]);
            if (ret1 == null && ret2 == null) continue;
            if (!ret1.passable && !ret2.passable)
            {
                result = false;
            }
        }

        return result;
    }
    
    public void HighLightOn(IEnumerable<Tile> tiles)
    {
        if (tiles == null) return;
        
        HighLightOff();
        foreach (var tile in tiles)
        {
            tile.Highlight = true;
            _highlightQueue.Enqueue(tile);
        }
    }

    private void HighLightOff()
    {
        while (_highlightQueue.TryDequeue(out var tile))
        {
            tile.Highlight = false;
        }
    }

    public void HighLightOn(Tile tile)
    {
        if (tile == null) return;
        
        HighLightOff();
        _highlightQueue.Enqueue(tile);
    }
    
    //demo code : 간단한 맵 생성용
    [Header("Creating Demo World Inspector")]
    public int range;
    
    [ContextMenu("Create World With Wall")]
    private void CreateDemoWorld()
    {
        var positions = Hex.GetGridsWithRange(range, Hex.zero);
        foreach (var pos in positions)
        {
            var isWall = Random.Range(0, 2) == 1;
            AddTile(pos, reachable : isWall, passable : isWall);
        }
    }
    
    [ContextMenu("Create World no wall")]
    private void CreateDemoWorld2()
    {
        var positions = Hex.GetGridsWithRange(range, Hex.zero);
        foreach (var pos in positions)
        {
            AddTile(pos);
        }
    }

    [ContextMenu("Remove Demo World")]
    private void RemoveDemoWorld()
    {
        _highlightQueue.Clear();
        foreach (var tile in _tiles)
        {
            DestroyImmediate(tile.Value.gameObject);
        }
        _tiles.Clear();
    }
    //demo end
}

/// <summary>
/// Path finding에 사용되는 Node 클래스
/// </summary>
internal class PathNode
{
    public PathNode(Vector3Int position,int g=0, int h=0, PathNode from=null)
    {
        this.pos = position;
        this.from = from;
    }
    
    public int G, H;
    public int F => G + H;

    public Vector3Int pos;
    public readonly PathNode from;
}