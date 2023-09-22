using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterTooltip : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _tooltipNameText;

    public bool isMouseOver { get; private set; }
    public string nameText { get; private set; }
    void Start()
    {
        isMouseOver = false;
        CloseUI();
    }

    public override void CloseUI()
    {
        nameText = "";
        base.CloseUI();
    }
    public void SetCharacterTooltip(CharacterStatUIInfo info, float yPosition)
    {
        OpenUI();
        nameText = info.statName;
        _tooltipNameText.GetComponent<TextMeshProUGUI>().text = info.GetTranslateStatName();
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.y = yPosition;
        GetComponent<RectTransform>().position = pos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        UIManager.instance.characterUI.characterStatUI.CloseCharacterTooltip(name);
    }
}
