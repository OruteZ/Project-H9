using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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
    /// <param name="limitMovement">최대 이동 칸 수</param>
    /// <returns>도달 가능한 모든 Tile들이 담긴 List</returns>
    public List<Tile> GetReachableTiles(Vector3Int start, int limitMovement)
    {
        var visited = new HashSet<Vector3Int> { start };
        var result = new List<Tile>() { GetTile(start) };
        var container = new Queue<Vector3Int>();
        container.Enqueue(start);

        for(int cnt = 0; cnt < limitMovement; cnt++)
        {
            int length = container.Count;
            for(int i = 0; i < length; i++)
            {
                if (!container.TryDequeue(out var current)) return result;

                foreach (var dir in Hex.directions)
                {
                    var next = current + dir;
                    if (visited.Contains(next)) continue;
                    if (!GetTile(next).reachable) continue;
                    
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
    /// </summary>
    /// <param name="start">시작지점</param>
    /// <param name="destination">도착지점</param>
    /// <returns>경로를 담은 리스트</returns>
    public LinkedList<Vector3Int> FindPath(Vector3Int start, Vector3Int destination)
    {
        //todo : 마저 구현
        var toSearch = new List<PathNode>()
        {
            new PathNode(0, Hex.Distance(start, destination), start)
        };
        var processed = new List<PathNode>();
        if (processed == null) throw new ArgumentNullException(nameof(processed));

        while (toSearch.Count > 0)
        {
            var current = toSearch[0];
            foreach (var n in toSearch)
            {
                current = current.F < n.F ? current : n;
            }
            
            processed.Add(current);
            toSearch.Remove(current);

            foreach (var dir in Hex.directions)
            {
                //var next = 
            }
        }

        return new LinkedList<Vector3Int>();
    }

    public void HighLightOn(IEnumerable<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.Highlight = true;
            _highlightQueue.Enqueue(tile);
        }
    }

    public void HighLightOff()
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
            Debug.Log(pos);
            AddTile(pos, reachable: Random.Range(0, 2) == 1);
        }
    }

    [ContextMenu("Remove Demo World")]
    private void RemoveDemoWorld()
    {
        foreach(var tile in _tiles) DestroyImmediate(tile.Value.gameObject);
    }
    //demo end
}

/// <summary>
/// Path finding에 사용되는 Node 클래스
/// </summary>
[Serializable]
internal class PathNode
{
    public PathNode(int g, int h, Vector3Int position)
    {
        pos = position;
        from = null;
    }
    
    public int G, H;
    public int F => G + H;

    public Vector3Int pos;
    [Serialize]
    public PathNode from;
}