using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
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

    [Header("플레이어 시야")] 
    [SerializeField] 
    private bool _inSight;

    public List<TileObject> interactiveObjects;
    public List<MeshRenderer> environments;

    public void AddObject(TileObject u)
    {
        interactiveObjects.Add(u);
    }

    public void RemoveObject(TileObject u)
    {
        interactiveObjects.Remove(u);
    }

    public bool inSight
    {
        get => _inSight;
        set
        {
            _inSight = value;
            
            SetObjectsSight();
            SetEnvironmentsSight();

            var unit = FieldSystem.unitSystem.GetUnit(hexPosition);
            if (unit is not null) unit.isVisible = value;
        }
    }

    private void SetObjectsSight()
    {
        for (var index = 0; index < interactiveObjects.Count; index++)
        {
                
            var obj = interactiveObjects[index];
            if (obj is FogOfWar fow)
            {
                fow.SetVisible(_inSight);
                if (_inSight) index--;
            }
                
                
            //Combat일때만 value 따라감
            else if (GameManager.instance.CompareState(GameState.Combat))
            {
                obj.SetVisible(_inSight);
            }
            //월드Scene일 경우 무조건 true
            else
            {
                obj.SetVisible(true);
            }
        }
    }

    private void SetEnvironmentsSight()
    {
        for (var index = 0; index < environments.Count; index++)
        {
            var obj = environments[index];
            if (GameManager.instance.CompareState(GameState.Combat))
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
            else
            {
                if (obj.materials.Length == 1) continue;

                Material mat = obj.material;
                obj.materials = new [] { mat };
            }
        }
    }
}