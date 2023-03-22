using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Material black;
    [ContextMenu("Get Range")]
    void GetRange()
    {
        var tiles = world.GetReachableTiles(start1, range);
        foreach(var t in tiles)
        {
            t.GetComponent<MeshRenderer>().material = black;
        }
    }

    private void OnValidate()
    {
        startMarker.position = start;
        endMarker.position = end;
        
        startMarker.OnValidate();
        endMarker.OnValidate();
    }
}
