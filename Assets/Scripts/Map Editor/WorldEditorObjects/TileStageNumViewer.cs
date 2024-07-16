using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TileStageNumViewer : MonoBehaviour
{
    public WorldData worldData;
    public Canvas canvas;
    public GameObject tileStageNumPrefab;

    [ReadOnly, SerializeField] 
    public Stack<WorldTextUpdater> numObjs = new ();

    private void Reload()
    {
        // destroy all canvases childre
        foreach (Transform child in canvas.transform)
        {
            Destroy(child.gameObject);
        }
        
        if (worldData == null)
        {
            Debug.LogError("WorldData not found");
            return;
        }
        
        foreach (Tile tile in FieldSystem.tileSystem.GetAllTiles())
        {
            GameObject tileStageNum = Instantiate(tileStageNumPrefab, canvas.transform);
            WorldTextUpdater text = tileStageNum.GetComponent<WorldTextUpdater>();

            if (text == null)
            {
                Debug.LogError("TileStageNumViewer not found");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
                #endif
            }

            text.hexPosition = tile.hexPosition;
            text.message = tile.combatStageIndex.ToString();
        }
    }
    
    private void Awake()
    {
        //Find WorldEditorViewer
        EditorMouseController controller = FindObjectOfType<EditorMouseController>();
        if (controller == null)
        {
            Debug.LogError("WorldEditorViewer not found");
            return;
        }
        
        //Subscribe to OnSelectedTileChanged event
        controller.onCommandExecuted.AddListener(Reload);
    }

    private void Start()
    {
        Reload();
    }
}