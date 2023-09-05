using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _envParent;
    [SerializeField] private GameObject _tileParent;

    public GameObject envTile;

    [ContextMenu("Generate Env")]
    private void Create()
    {
        Remove();
        
        var tiles = _tileParent.GetComponentsInChildren<Tile>();

        foreach (var tile in tiles)
        {
            EnvObject envObject = Instantiate(envTile, _envParent.transform).GetComponent<EnvObject>();

            // var worldPos = envObject.transform.position;
            // envObject.transform.position = worldPos;
            
            envObject.hexPosition = tile.hexPosition;
            envObject.OnSetting();

            envObject.GetComponent<MeshRenderer>().material.color = 
                new Color(tile.walkable ? 1 : 0, tile.visible ? 1 : 0, tile.rayThroughable ? 1 : 0);
        }   
    }

    [ContextMenu(("Remove Env"))]
    private void Remove()
    {
        var envs = _envParent.GetComponentsInChildren<HexTransform>();
        foreach(var env in envs)
        {
            DestroyImmediate(env.gameObject);
        }
    }
}
