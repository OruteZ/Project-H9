using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public void AddTile(Vector3Int position, bool reachable = true)
    {
        var tile = Instantiate(tileObject, transform).GetComponent<Tile>();
        tile.hexTransform.position = position;
        tile.reachable = reachable;
        _tiles.Add(position, tile);
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
                    if (!tile.reachable) continue;
                    
                    result.Add(GetTile(next));
                    container.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return result;
    }

    //todo: findpath함수 최적화 (모든 타일에 class생성은 상당히 비합리적) from 배열 하나 생성해서 처리하는 작업이 필요할 것으로 보임

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
                    if (!tile.reachable) continue;
                    
                    container.Enqueue(new PathNode(next, from:current));
                    visited.Add(next);
                }
            }
        }

        return null;
    }
    // public IEnumerable<Tile> FindPath(Vector3Int start, Vector3Int destination, int maxLength=3)
    // {
    //     var toSearch = new List<PathNode>()
    //     {
    //         new(0, Hex.Distance(start, destination), start)
    //     };
    //     var processed = new List<PathNode>();
    //
    //     while (toSearch.Count > 0)
    //     {
    //         var current = toSearch[0];
    //         foreach (var node in toSearch)
    //         {
    //             if (node.F < current.F) current = node;
    //             else if (current.F == node.F && node.H < current.H) current = node;
    //         }
    //         processed.Add(current);
    //         toSearch.Remove(current);
    //
    //         if (current.pos == destination)
    //         {
    //             var path = new List<Tile>();
    //             while (current.from != null)
    //             {
    //                 path.Add(GetTile(current.pos));
    //                 current = current.from;
    //             }
    //             path.Add(GetTile(start));
    //             return path;
    //         }
    //
    //         foreach (var dir in Hex.directions)
    //         {
    //             var nextPosition = current.pos + dir;
    //             Debug.Log("next pos = " + nextPosition);
    //             if (processed.Any(n => n.pos == nextPosition)) continue;
    //             if (GetTile(nextPosition) == null) continue;
    //             if (GetTile(nextPosition).reachable == false) continue;
    //             
    //             PathNode nextNode = null;
    //             foreach (var node in toSearch)
    //             {
    //                 if (node.pos == nextPosition)
    //                 {
    //                     nextNode = node;
    //                     break;
    //                 }
    //             }
    //             bool inSearch = nextNode != null;
    //             int costToNext = current.G + 1;
    //
    //             if (!inSearch)
    //             {
    //                 nextNode = new PathNode(
    //                     costToNext, 
    //                     Hex.Distance(nextPosition, destination),
    //                     nextPosition
    //                 ).from = current;
    //             }
    //             else if (costToNext < nextNode.G)
    //             {
    //                 nextNode.G = costToNext;
    //                 nextNode.from = current;
    //             }
    //             
    //             if(!inSearch && nextNode.F <= maxLength)
    //                 toSearch.Add(nextNode);
    //         }
    //     }
    //     return null;
    // }

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
    
    //demo code : 간단한 맵 생성용
    [Header("Creating Demo World Inspector")]
    public int range;
    
    [ContextMenu("Create World")]
    private void CreateDemoWorld()
    {
        var positions = Hex.GetGridsWithRange(range, Hex.zero);
        foreach (var pos in positions)
        {
            AddTile(pos, reachable: Random.Range(0, 2) == 1);
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
    public PathNode from;
}