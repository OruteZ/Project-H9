using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour, IEquatable<Tile>
{
    private HexTransform _hexTransform;

    public Vector3Int hexPosition
    {
        get
        {
            _hexTransform ??= GetComponent<HexTransform>();
            return _hexTransform.position;
        }
        set
        {
            _hexTransform ??= GetComponent<HexTransform>();
            _hexTransform.position = value;
        } 
    }

    [Header("타일 속성")] 
    public bool walkable;
    public bool visible;
    public bool rayThroughable;
    public bool gridVisible;
    public int combatStageIndex;

    [Header("플레이어 시야")] 
    [SerializeField] 
    private bool _inSight;

    public List<TileObject> tileObjects;
    public List<MeshRenderer> environments;

    public void AddObject(TileObject u)
    {
        tileObjects.Add(u);
    }

    public void RemoveObject(TileObject u)
    {
        tileObjects.Remove(u);
    }

    public bool inSight
    {
        get => _inSight;
        set
        {
            _inSight = value;
            
            SetObjectsSight();
            SetEnvironmentsSight();
        }
    }

    private void SetObjectsSight()
    {
        for (var index = 0; index < tileObjects.Count; index++)
        {
            var obj = tileObjects[index];
            if (obj is FogOfWar fow)
            {
                fow.SetVisible(_inSight);
                if (_inSight) index--;
            }
            
            else obj.SetVisible(_inSight);
        }
    }

    private void SetEnvironmentsSight()
    {
        for (var index = 0; index < environments.Count; index++)
        {
            var obj = environments[index];
            if (GameManager.instance.CompareState(GameState.COMBAT))
            {
                if (_inSight is false)
                {
                    if (obj.materials.Length >= 2) continue;
                    
                    var matList = obj.materials.ToList();
                    matList.Add(TileEffectManager.instance.combatFowMaterial);

                    obj.materials = matList.ToArray();
                }
                else
                {
                    if (obj.materials.Length == 1) continue;

                    Material mat = obj.material;
                    obj.materials = new [] { mat };
                }
            }
            else //World Scene
            {
                if (obj.materials.Length == 1) continue;

                Material mat = obj.material;
                obj.materials = new [] { mat };
            }
        }
    }

    public bool Equals(Tile other)
    {
        return other != null && hexPosition == other.hexPosition;
    }

    public T GetTileObject<T>() where T : TileObject
    {
        return tileObjects.Find(obj => obj is T) as T;
    }
    
    public T[] GetTileObjects<T>() where T : TileObject
    {
        return tileObjects.FindAll(obj => obj is T).Cast<T>().ToArray();
    }
}