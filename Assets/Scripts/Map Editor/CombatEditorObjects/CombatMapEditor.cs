using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CombatMapEditor : MonoBehaviour
{
    public LinkDatabase linkDatabase;
    
    public CombatStageData mapData;
    public int currentStageIndex;

    public readonly HashSet<Vector3Int> spawnPoints = new ();
    public Vector3Int playerSpawnPoint;

    public UnityEvent onPointsChanged = new();
    
    public TMP_InputField inputLinkIndex;
    public TMP_InputField inputStageIndex;

    private void Start()
    {
        SetSpawnPositions();
    }

    public void Save()
    {
        mapData.SetEnemyPoints(spawnPoints.ToArray());
        mapData.SetPlayerPoint(playerSpawnPoint);

        onPointsChanged.Invoke();
    }
    
    public void SetLinkIndex()
    {
        // if (int.TryParse(inputLinkIndex.text, out int index))
        // {
        //     if (index == maxSpawnsCount)
        //     {
        //         inputLinkIndex.text = index.ToString();
        //         return;
        //     }
        //     ChangeLink(index);
        // }
        // else
        // {
        //     inputLinkIndex.text = maxSpawnsCount.ToString();
        // }
    }
    
    public void SetStageIndex()
    {
        if (int.TryParse(inputStageIndex.text, out var index))
        {
            CombatStageData data = Resources.Load<CombatStageData>("Map Data/Stage " + index);
            if (data is null)
            {
                inputStageIndex.text = currentStageIndex.ToString(); 
                return;
            }
            ChangeMap(data);
        }
        else
        {
            inputStageIndex.text = currentStageIndex.ToString();
        }
    }

    private void SetSpawnPositions()
    {
        //set player spawn
        playerSpawnPoint = mapData.TryGetPlayerPoint(out Vector3Int playerPoint)
            ? playerPoint : Hex.zero;

        //set enemies spawn
        spawnPoints.Clear();
        mapData.TryGetAllEnemyPoints(out var points);
            
        foreach (Vector3Int point in points)
        {
            spawnPoints.Add(point);
        }
        
        onPointsChanged.Invoke();
    }
    
    // private void ChangeLink(int linkIndex)
    // {
    //     SetSpawnPositions();
    // }

    private void ChangeMap(CombatStageData data)
    {
        mapData = data;
        
        MapSaveAndLoader loader = FindObjectOfType<MapSaveAndLoader>();
        loader.saveData = data;
        loader.LoadMap();

        SetSpawnPositions();
    }
}