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
            HexTransform envObject = Instantiate(envTile, _envParent.transform).GetComponent<HexTransform>();
            envObject.name = $"Ground_{tile.hexPosition.x}_{tile.hexPosition.y}_{tile.hexPosition.z}";

            // var worldPos = envObject.transform.position;
            // envObject.transform.position = worldPos;
            
            envObject.position = tile.hexPosition;
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
