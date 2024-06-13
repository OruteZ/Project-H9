using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjDatabase", menuName = "ScriptableObjects/Tile Object Database", order = 1)]
public class TileObjDatabase : ScriptableObject
{
    public List<GameObject> tileObjects;  
    
    public GameObject GetTileObject(TileObjectType type)
    {
        return tileObjects.FirstOrDefault(obj => obj.GetComponent<TileObject>().objectType == type);
    }
}