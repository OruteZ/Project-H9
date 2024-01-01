using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CombatMapEditor : MonoBehaviour
{
    public LinkDatabase linkDatabase;
    public int currentLinkIndex;
    
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
    
    public LinkData GetCurrentLink()
    {
        return linkDatabase.GetData(currentLinkIndex);
    }

    public void Save()
    {
        mapData.SetEnemyPoints(currentLinkIndex, spawnPoints.ToArray());
        mapData.SetPlayerPoint(currentLinkIndex, playerSpawnPoint);

        onPointsChanged.Invoke();
    }
    
    public void SetLinkIndex()
    {
        if (int.TryParse(inputLinkIndex.text, out var index))
        {
            if (index == currentLinkIndex)
            {
                inputLinkIndex.text = index.ToString();
                return;
            }
            ChangeLink(index);
        }
        else
        {
            inputLinkIndex.text = currentLinkIndex.ToString();
        }
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
        playerSpawnPoint = mapData.TryGetPlayerPoint(currentLinkIndex, out var playerPoint)
            ? playerPoint : Hex.zero;

        //set enemies spawn
        spawnPoints.Clear();
        if (mapData.TryGetEnemyPoints(currentLinkIndex, out var points))
        {
            foreach (var point in points)
            {
                spawnPoints.Add(point);
            }
        }
        
        onPointsChanged.Invoke();
    }
    
    private void ChangeLink(int linkIndex)
    {
        currentLinkIndex = linkIndex;
        SetSpawnPositions();
    }

    private void ChangeMap(CombatStageData data)
    {
        mapData = data;
        
        var loader = FindObjectOfType<MapSaveAndLoader>();
        loader.saveData = data;
        loader.LoadMap();

        SetSpawnPositions();
    }
}