using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectUI : UISystem
{
    [SerializeField] private GameObject _mouseOverIcon;
    [SerializeField] private GameObject _tileObjectTooltip;

    public override void OpenPopupWindow()
    {
        UIManager.instance.SetUILayer(3);
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.SetUILayer(1);
    }
    private void Update()
    {
        if (!GameManager.instance.CompareState(GameState.COMBAT)) return;
        if (FieldSystem.turnSystem.turnOwner is Player)
        {
            TileObject tObj = GetMouseOverOnTileObject(out Vector3Int ep);
            if (tObj == null || tObj.objectType == TileObjectType.NONE) 
            {
                _mouseOverIcon.SetActive(false);
                _tileObjectTooltip.GetComponent<TileObjectTooltip>().CloseUI();
                return;
            }

            if (tObj.objectType == TileObjectType.BEER ||
                tObj.objectType == TileObjectType.OIL_BARREL ||
                tObj.objectType == TileObjectType.TNT_BARREL ||
                tObj.objectType == TileObjectType.TRAP)
            {
                _mouseOverIcon.SetActive(true);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(FieldSystem.tileSystem.GetTile(ep).transform.position);
                _mouseOverIcon.GetComponent<RectTransform>().position = screenPos;

                if (Input.GetMouseButtonDown(1))
                {
                    _tileObjectTooltip.GetComponent<TileObjectTooltip>().SetTileObjectUITooltip(tObj.objectType, screenPos);
                }
            }
        }
        else
        {
            _mouseOverIcon.SetActive(false);
            _tileObjectTooltip.GetComponent<TileObjectTooltip>().CloseUI();
        }
    }
    private static TileObject GetMouseOverOnTileObject(out Vector3Int pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isSuccessRaycast = Physics.Raycast(ray, out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Tile"));
        if (isSuccessRaycast && hit.collider.TryGetComponent<Tile>(out var tile))
        {
            List<TileObject> tObjs = FieldSystem.tileSystem.GetTileObject(tile.hexPosition);
            if (tObjs.Count > 0 && tObjs[0].IsVisible())
            {
                pos = tObjs[0].hexPosition;
                return tObjs[0];
            } 
        }

        pos = Vector3Int.zero;
        return null;
    }
}
