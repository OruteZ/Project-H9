using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Actor : TileObject
{
    //한칸을 이동하는데 걸리는 시간입니다.
    public float oneTileMoveTime;
    public WorldMap world;

    [Header("For Debug")]
    [SerializeField] private ActorState state;

    [Header("Control Inspector")] 
    [SerializeField] private int speed;
    
    
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        hexTransform.position = Vector3Int.zero;
        state = ActorState.ReadyToAct;
    }

    private void Move(Vector3Int destination)
    {
        var route = world.FindPath(hexTransform.position, destination);
        if (route == null) return;
        
        StartCoroutine(MoveCoroutine(route));
    }
    private IEnumerator MoveCoroutine(IEnumerable<Tile> route)
    {
        state = ActorState.Busy;
        var oneDivTileMoveTime = 1 / oneTileMoveTime;

        foreach (var dest in route)
        {
            if (dest.hexTransform.position == hexTransform.position) continue;
            
            var start = Hex.Hex2World(hexTransform.position);
            var end = Hex.Hex2World(dest.hexTransform.position);
            var time = 0f;
            while (time <= oneTileMoveTime)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, time * oneDivTileMoveTime);
                yield return null;
            }

            hexTransform.position = dest.hexTransform.position;
        }

        state = ActorState.ReadyToAct;
    }
    private IEnumerator MoveCoroutine(IEnumerable<Vector3Int> route)
    {
        var oneDivTileMoveTime = 1 / oneTileMoveTime;

        foreach (var dest in route)
        {
            if (dest == hexTransform.position) continue;
            
            var start = Hex.Hex2World(hexTransform.position);
            var end = Hex.Hex2World(dest);
            var time = 0f;
            while (time <= oneTileMoveTime)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, time * oneDivTileMoveTime);
                yield return null;
            }

            hexTransform.position = dest;
        }
    }

    private void Update()
    {
        if (state == ActorState.Busy) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit hit; 
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var dest = hit.transform.GetComponent<HexTransform>().position;
                Move(dest);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit; 
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var dest = hit.transform.GetComponent<HexTransform>().position;
                Move(dest);
            }
        }
    }
}

public enum ActorState
{
    Busy,
    ReadyToAct,
    Moving,
}
