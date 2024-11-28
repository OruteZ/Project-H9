using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileObjectTooltip : UIElement, IPointerExitHandler
{
    [SerializeField] private GameObject _objectNameText;
    [SerializeField] private GameObject _objectDescriptionText;

    private TileObjectType _currentTooltipType;

    private const int X_POSITION_CORRECTION = 50;
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }
    public void SetTileObjectUITooltip(TileObjectType tObjType, Vector3 pos)
    {
        if (_currentTooltipType == tObjType && GetComponent<RectTransform>().position == pos) return;

        if (tObjType == TileObjectType.NONE)
        {
            CloseUI();
            return;
        }
        _currentTooltipType = tObjType;
        pos.x += X_POSITION_CORRECTION * UIManager.instance.GetCanvasScale();
        GetComponent<RectTransform>().position = pos;

        UIManager.instance.SetUILayer(3);

        SetTileObjectTooltipText(tObjType);

        OpenUI();
    }
    private void SetTileObjectTooltipText(TileObjectType tObjType)
    {
        KeywordScript keywordScript = SkillManager.instance.GetSkillKeyword(TileTypeToKeywordIndex(tObjType));
        _objectNameText.GetComponent<TextMeshProUGUI>().text = keywordScript.name;
        _objectDescriptionText.GetComponent<TextMeshProUGUI>().text = keywordScript.description;
    }
    private int TileTypeToKeywordIndex(TileObjectType tObjType)
    {
        return tObjType switch
        {
            TileObjectType.TNT_BARREL => 10001,
            TileObjectType.OIL_BARREL => 10002,
            TileObjectType.TRAP => 10003,
            TileObjectType.BEER => 10004,
            _ => 0
        };
    }
    public override void CloseUI()
    {
        gameObject.SetActive(false);
    }
}