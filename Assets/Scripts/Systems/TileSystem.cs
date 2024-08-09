using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileSystem : MonoBehaviour
{
    [Header("Tile Data")] public string dataName;
    
    /// <summary>
    /// 타일 Prefab입니다.
    /// </summary>
    public GameObject tilePrefab;
    
    /// <summary>
    /// Link Prefab입니다.
    /// </summary>
    public GameObject linkPrefab;
    
    /// <summary>
    /// 전장의 안개 Prefab입니다.
    /// </summary>
    public GameObject worldFogOfWarPrefab;

    public GameObject combatFogOfWarPrefab;
    
    /// <summary>
    /// 모든 전장의 안개를 자식으로 가질 오브젝트 입니다.
    /// </summary>
    public Transform fogs;
    
    /// <summary>모든 TIle을 자식으로 가질 오브젝트입니다. </summary>
    public GameObject tileParent;

    public GameObject tileObjParent;
    
    /// <summary>
    /// 모든 환경요소를 자식으로 가지는 오브젝트입니다.
    /// </summary>
    public Transform environments;
    
    private Dictionary<Vector3Int, Tile> _tiles = new();
    private List<TileObject> _tileObjects = new();
    
    private HexGridLayout _gridLayout;
    private HexGridLayout gridLayout => _gridLayout ??= tileParent.GetComponent<HexGridLayout>();

    /// <summary>
    /// 현재 존재하는 모든 타일의 reference를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetAllTiles()
    {
        if (_tiles is null) return null;
        
        var result = new List<Tile>();
        result.AddRange(_tiles.Values);

        return result;
    }
    
    #if UNITY_EDITOR
    public List<Tile> GetAllTilesEditor()
    {
        var tilesInChildren = GetComponentsInChildren<Tile>();  
        var result = new List<Tile>();
        result.AddRange(tilesInChildren);

        return result;
    }
    #endif
    
    /// <summary>
    /// 현재 존재하는 모든 타일의 Hex 위치를 반환합니다.
    /// </summary>
    /// <returns>타일들의 위치목록</returns>
    public List<Vector3Int> GetAllTilePos()
    {
        var result = new List<Vector3Int>();
        result.AddRange(_tiles.Values.Select(x => x.hexPosition));

        return result;
    }

    /// <summary>
    /// 자식 오브젝트로 존재하는 Tile과 TileObject를 모두 관리하고, 전장의 안개를 생성합니다.
    /// 이 함수가 호출되기 전 까지 타일에 대한 접근은 Null Reference Exception을 유발합니다.
    /// </summary>
    public void SetUpTilesAndObjects()
    {
        _tiles = new Dictionary<Vector3Int, Tile>();
        _gridLayout = tileParent.GetComponent<HexGridLayout>();
        
        var tilesInChildren = GetComponentsInChildren<Tile>();  
        foreach (Tile t in tilesInChildren)
        {
            AddTile(t);
            if (GameManager.instance.CompareState(GameState.WORLD) && 
                GameManager.instance.IsPioneeredWorldTile(t.hexPosition) is false)
            {
                FogOfWar fow = Instantiate(worldFogOfWarPrefab, fogs).GetComponent<FogOfWar>(); 
                fow.hexPosition = t.hexPosition;
            }
        }
        
        //if world state
        if (GameManager.instance.CompareState(GameState.WORLD))
        {
            // get all specific tile index values
            var infos = GameManager.instance.runtimeWorldData.specificCombatIndexedTiles;

            foreach (TileCombatStageInfo info in infos)
            {
                var tile = GetTile(info.hexPosition);
                if (tile is null) continue;
                
                tile.combatStageIndex = info.combatStageIndex;
            }
        }
        

        var objects = GetComponentsInChildren<TileObject>().ToList();
        foreach (TileObject obj in objects)
        {
            obj.SetUp();
            _tileObjects.Add(obj);
        }
        
        //get runtime map data from gamemanager
        var mapData = GameManager.instance.runtimeWorldData;
        if (mapData is not null)
        {
            if(GameManager.instance.CompareState(GameState.WORLD)) 
                foreach (LinkObjectData link in mapData.links)
                {
                    AddLink(link.pos, link.rotation, link.linkIndex, link.combatMapIndex);
                }
        }

        var envList = environments.GetComponentsInChildren<HexTransform>().ToList();
        foreach (var env in envList)
        {
            var pos = env.position;
            var tile = GetTile(pos);
            
            tile.environments.Add(env.GetComponent<MeshRenderer>());
        }
        
        foreach (Tile t in GetAllTiles())
        {
            t.inSight = GameManager.instance.IsPioneeredWorldTile(t.hexPosition) && 
                        GameManager.instance.CompareState(GameState.WORLD);
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
    /// Runtime에 Link를 추가합니다.
    /// </summary>
    public void AddLink(Vector3Int position, float rotation, int linkIndex, int mapIndex = 1, bool isRepeatable = false)
    {
        
        //if link that has same position with tile already exist, skip
        var tile = GetTile(position);
        if (tile is null)
        {
            Debug.LogError("Link를 추가할 타일이 없습니다.");
            return;
        }

        if (tile.tileObjects.Any(obj => obj is Link))
        {
            Debug.LogError("이미 Link가 있는 타일에 Link를 추가하려고 합니다." +
                           "pos : " + position + 
                           ", LinkIndex : " + linkIndex + 
                           ", MapIndex : " + mapIndex
                           );
            return;
        }
        
        var obj = Instantiate(linkPrefab, tileObjParent.transform).GetComponent<Link>();
        obj.hexPosition = position;
        obj.transform.rotation = Quaternion.Euler(0, rotation, 0);
        obj.linkIndex = linkIndex;
        obj.combatMapIndex = mapIndex;
        obj.isRepeatable = isRepeatable;
        obj.SetUp();
        
        _tileObjects.Add(obj);
    }

    /// <summary>
    /// 해당 Hex좌표에 해당하는 Tile을 가져옵니다.
    /// </summary>
    /// <param name="position">Hex좌표</param>
    /// <returns>Tile</returns>
    public Tile GetTile(Vector3Int position)
    {
        return _tiles.GetValueOrDefault(position);
    }

    #if UNITY_EDITOR
    public Tile GetTileInEditor(Vector3Int position)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("This Function Only Called in editor");
            return null;
        }
        
        var tilesInChildren = GetComponentsInChildren<Tile>();  
        foreach (Tile t in tilesInChildren)
        {
            if (t.hexPosition == position) return t;
        }
        
        return null;
    }
    #endif
    
    /// <summary>
    /// 해당 Hex좌표에 존재하는 모든 TileObject를 가져옵니다.
    /// </summary>
    /// <param name="position">Hex 좌표</param>
    /// <returns></returns>
    public List<TileObject> GetTileObject(Vector3Int position)
    {
        return _tileObjects.FindAll(obj => obj.hexPosition == position);
    }
    
    /// <summary>
    /// 존재하는 모든 TileObjects를 반환합니다.
    /// </summary>
    public IEnumerable<TileObject> GetAllTileObjects()
    {
        return _tileObjects;
    }
    public void DeleteTileObject(TileObject obj) 
    {
        _tileObjects.Remove(obj);
    }

    /// <summary>
    /// start지점에서 limitMovement 칸 이내에 도달 할 수 있는 모든 Tile을 반환합니다.
    /// </summary>
    /// <param name="start">이동 시작점</param>
    /// <param name="maxLength">최대 이동 칸 수</param>
    /// <returns>도달 가능한 모든 Tile들이 담긴 List</returns>
    public IEnumerable<Tile> GetWalkableTiles(Vector3Int start, int maxLength)
    {
        if (GetTile(start) is null)
        {
            Debug.Log("Start Tile is Null");
            return null;
        }
        
        var visited = new HashSet<Vector3Int> { start };
        var result = new List<Tile> { GetTile(start) };
        var container = new Queue<Vector3Int>();
        container.Enqueue(start);

        for(int cnt = 0; cnt < maxLength; cnt++)
        {
            int length = container.Count;
            for(int i = 0; i < length; i++)
            {
                if (!container.TryDequeue(out Vector3Int current)) return result;

                foreach (Vector3Int dir in Hex.directions)
                {
                    Vector3Int next = current + dir;
                    if (visited.Contains(next)) continue;
                    
                    Tile tile = GetTile(next);
                    if (tile is null) continue;
                    if (!tile.walkable) continue;
                    if (FieldSystem.unitSystem.GetUnit(tile.hexPosition) != null) continue;
                    
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
    
    public IEnumerable<Tile> GetTilesOutLine(Vector3Int start, int range_)
    {
        var list = Hex.GetCircleLineGridList(range_, start);
        
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
    /// 불가능한 경로는 null을 반환합니다.
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
                    if (FieldSystem.unitSystem.GetUnit(tile.hexPosition) is not null
                        && next != destination) continue;
                    
                    container.Enqueue(new PathNode(next, from:current));
                    visited.Add(next);
                }
            }
        }
        
        //Debug.Log("MaxLength is " + maxLength + ", Find Path is null");
        return null;
    }

    /// <summary>
    /// start 지점에서 target까지 Ray를 발사합니다. 가로막히는 벽의 기준은 ray-throughable 변수입니다.
    /// </summary>
    /// <param name="start">시작점의 좌표</param>
    /// <param name="target">목적지의 좌표</param>
    /// <returns>두 지점 사이 장애물이 없으면 true를 반환합니다. </returns>
    public bool RayThroughCheck(Vector3Int start, Vector3Int target)
    { 
        var line1 = Hex.DrawLine1 (start, target);
        var line2 = Hex.DrawLine2(start, target);

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
    public bool VisionCheck(Vector3Int start, Vector3Int target)
    { 
        var line1 = Hex.DrawLine1(start, target);
        var line2 = Hex.DrawLine2(start, target);

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

    //==========================Create World==================================
    enum CreateType
    {
        RECT,
        HEXAGON
    }
    
    
    [Header("Creating World")]
    [Header("Hexagon Setting")]
    [SerializeField] private int range;
    [SerializeField] private Vector3Int center;

    [Space(5)] 
    [Header("Rect Setting")] 
    [SerializeField] private Vector3Int start;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [Tooltip("Debug Red line's height")]
    [SerializeField] private int gizmoHeight;
    
    [SerializeField] private Transform tilesParent;
    [SerializeField] private CreateType createType;

    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        IEnumerable<Vector3Int> list = 
            createType == CreateType.HEXAGON ? 
                Hex.GetCircleGridList(range, center) :
                Hex.GetSquareGridList(width, height, start);
        
        
        
        IEnumerable<Tile> tiles = GetAllTilesEditor();

        // only positions that not in tiles
        IEnumerable<Vector3Int> positions = list.Where(pos => tiles.All(tile => tile.hexPosition != pos));
        
        foreach (Vector3Int pos in positions)
        {
            Tile tile = Instantiate(tilePrefab, tilesParent).GetComponent<Tile>();
            
            tile.hexPosition = pos;
            tile.walkable = true;
            tile.visible = true;
            tile.rayThroughable = true;
            tile.gridVisible = true;
            
            tile.gameObject.name = "Tile : " + pos;
        }
        
        gridLayout.LayoutGrid();
    }

    [SerializeField] private bool viewGeneratorCenter;
    private void OnDrawGizmos()
    {
        if (viewGeneratorCenter)
        {
            if(createType == CreateType.HEXAGON)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Hex.Hex2World(center), range * 2);
            }
            else
            {
                if(width == 0 || height == 0) return;
                
                Gizmos.color = Color.red;
                Vector3 leftTop = Hex.Hex2World(start);
                
                var gridList = Hex.GetSquareGridList(width, height, start);
                Vector3 rightBottom = gridList.Count > 0 ?
                    Hex.Hex2World(gridList.Last()) :
                    Hex.Hex2World(start);
                
                Vector3 c = (leftTop + rightBottom) / 2;
                
                Gizmos.DrawWireCube(c, rightBottom - leftTop + new Vector3(0, gizmoHeight, 0));
            }
        }
    }
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