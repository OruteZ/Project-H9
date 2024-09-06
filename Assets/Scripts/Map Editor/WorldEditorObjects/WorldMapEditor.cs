using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class WorldMapEditor : MonoBehaviour
{
    [SerializeField]private WorldData worldData;
    [SerializeField]private Transform tileObjectsTransform;
    [SerializeField]private GameObject linkPrefab;
    
    [HideInInspector] public UnityEvent onLinkChanged = new UnityEvent();
    
    private void Awake()
    {
        //Find TileObjects;
        GameObject tileObjects = GameObject.Find("TileObjects");
        if (tileObjects == null)
        {
            Debug.LogError("TileObjects not found");
            return;
        }
        tileObjectsTransform = tileObjects.transform;
        
        UIManager.instance.gameObject.SetActive(false);
        GameManager.instance.SetEditor();
        TileEffectManager.instance.gameObject.SetActive(false);
        
        LoadData();
    }

    [ContextMenu("Load Link")]
    private void LoadData()
    {
        //Clear link objects
        var links = FindObjectsOfType<Link>();
        foreach (Link link in links)
        {
            DestroyImmediate(link.gameObject);
        }
        
        //Instantiate link object by link data
        foreach (LinkObjectData linkData in worldData.links)
        {
            //instantiate
            GameObject link = Instantiate(linkPrefab, Hex.Hex2World(linkData.pos), Quaternion.identity);
            link.transform.SetParent(tileObjectsTransform);
            link.transform.rotation = Quaternion.Euler(0, linkData.rotation, 0);
            
            Link linkComponent = link.GetComponent<Link>();
            linkComponent.linkIndex = linkData.linkIndex;
            linkComponent.combatMapIndex = linkData.combatMapIndex;
            linkComponent.isRepeatable = linkData.isRepeatable;

            if (link.TryGetComponent(out linkComponent.hexTransform))
            {
                linkComponent.hexPosition = linkData.pos;
            }
        }
        
        // combat indexed tiles
        foreach (TileCombatStageInfo info in worldData.specificCombatIndexedTiles)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(info.hexPosition);
            if (tile == null)
            {
                Debug.LogError("Cannot Found tile that has position " + info.hexPosition.ToString());
                continue;
            }
            
            tile.combatStageIndex = info.combatStageIndex;
        }
    }
    
    public void SaveData()
    {
        //Get link data by link object
        worldData.links.Clear();
        foreach (Transform child in tileObjectsTransform)
        {
            Link link = child.GetComponent<Link>();
            if (link == null) continue;
            
            LinkObjectData linkData = new LinkObjectData
            {
                pos = link.hexPosition,
                rotation = child.transform.rotation.eulerAngles.y,
                linkIndex = link.linkIndex,
                combatMapIndex = link.combatMapIndex,
                isRepeatable = link.isRepeatable,
                model = null
            };
            worldData.links.Add(linkData);
        }
        
        //Get tile data
        worldData.specificCombatIndexedTiles.Clear();
        foreach(Tile tile in FieldSystem.tileSystem.GetAllTiles())
        {
            if (tile.combatStageIndex == -1) continue;
            
            Vector3Int pos = tile.hexPosition;
            int combatMapIndex = tile.combatStageIndex;

            
            //if already exist, skip
            if (worldData.specificCombatIndexedTiles.
                Exists(x => x.hexPosition == pos)) continue;
            
            worldData.specificCombatIndexedTiles.Add(new TileCombatStageInfo
            {
                combatStageIndex = combatMapIndex,
                hexPosition = pos
            });
        }
        
        
        WorldData.SaveChangesToScriptableObject(worldData);
        onLinkChanged.Invoke();
    }

    #if UNITY_EDITOR
    [ContextMenu("Save Tile Combat data")]
    public void LoadTileCombatData()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("This function is only available in editor mode");
            return;
        }

        TileSystem tileSystem = FindObjectOfType<TileSystem>();
        foreach (TileCombatStageInfo tileCombatStageInfo in worldData.specificCombatIndexedTiles)
        {
            Tile tile = tileSystem.GetTileInEditor(tileCombatStageInfo.hexPosition);
            if (tile == null)
            {
                Debug.LogError("Cannot Found tile that has position " + tileCombatStageInfo.hexPosition.ToString());
            }
            tile.combatStageIndex = tileCombatStageInfo.combatStageIndex;
        }
    }
    #endif
}