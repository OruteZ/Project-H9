using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 각종 기능 테스트를 위한 콘솔 대용 오브젝트입니다.
/// </summary>
public class ConsoleForTest : MonoBehaviour
{
    [Header("LineDraw Inspector")]
    public Vector3Int start;
    public Vector3Int end;
    public GameObject markerPrefab;
    public HexTransform startMarker;
    public HexTransform endMarker;
    [ContextMenu("LineDraw Test")]
    void LineDrawTest()
    {
        var line = Hex.LineDraw(start, end);
        foreach (var l in line)
        {
            Debug.Log(l);
            Instantiate(markerPrefab, Hex.Hex2World(l), Quaternion.identity);
        }
    }

    [Header("Get Range Inspector")]
    public Vector3Int start1;
    public int range;
    public WorldMap world;
    [ContextMenu("Get Range")]
    void GetRange()
    {
        var tiles = world.GetReachableTiles(start1, range);
        world.HighLightOn(tiles);
    }

    [Header("Find Path Tester")]
    public Vector3Int startPoint;
    public Vector3Int destPoint;

    [ContextMenu("Find Path")]
    void FindPath()
    {
        world.GetTile(startPoint).Highlight = true;
        world.GetTile(destPoint).Highlight = true;
        
        var path = world.FindPath(startPoint, destPoint);
        Debug.Log(path);
        world.HighLightOn(path);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(Input.mousePosition);
            
            RaycastHit hit; 
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                startPoint = hit.transform.GetComponent<HexTransform>().position;
            }
        }
        
        RaycastHit hit2; 
        var ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray2, out hit2))
        {
            var path = world.FindPath(startPoint, hit2.transform.GetComponent<HexTransform>().position);
            world.HighLightOn(path);
        }
    }
}
