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

    private CharacterStatTextElement _textElement;

    [SerializeField] private GameObject[] _CharacterStatTooltipTexts;
    void Start()
    {
        isMouseOver = false;
        nameText = "";
        _textElement = null;
        CloseUI();
    }
    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        if (_textElement is null)
        {
            base.CloseUI();
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(DeleyedCloseCharacterTooltip());
        }
    }

    IEnumerator DeleyedCloseCharacterTooltip()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            if (isMouseOver) yield break;
            if (_textElement.isMouseOver) yield break;
            nameText = "";
            base.CloseUI();
            yield break;
        }
    }
    public void SetCharacterTooltip(CharacterStatTextElement textElement, CharacterStatUIInfo info, float yPosition)
    {
        if (nameText == info.statName) return;

        //Tooltip setting
        OpenUI();
        nameText = info.statName;
        _textElement = textElement;
        _tooltipNameText.GetComponent<TextMeshProUGUI>().text = info.GetTranslateStatName();
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.y = yPosition;
        GetComponent<RectTransform>().position = pos;


        //Text Setting
        foreach (GameObject texts in _CharacterStatTooltipTexts) 
        {
            texts.GetComponent<CharacterTooltipText>().CloseUI();
        }
        if (info.statName == "Name") return;
        bool isExistCharacterStat = (info.statValues["CharacterStat"] != 0);
        bool isExistWeaponStat = (info.statValues["WeaponStat"] != 0);
        bool isExistSkillStat = (info.statValues["SkillStat"] != 0);
        bool[] isExist =
        {
            isExistCharacterStat,
            (isExistCharacterStat && (isExistWeaponStat || isExistSkillStat)), 
            isExistWeaponStat,
            ((isExistCharacterStat || isExistWeaponStat) && (isExistSkillStat)),
            isExistSkillStat 
        };
        List<(string, float)> tooltipTexts = new List<(string, float)>()
        {
            ( "CharacterStat", info.GetCorrectedValue(info.statValues["CharacterStat"]) ),
            ( "", 11 ),
            ( "WeaponStat", info.GetCorrectedValue(info.statValues["WeaponStat"]) ),
            ( "", 11 ),
            ( "SkillStat", info.GetCorrectedValue(info.statValues["SkillStat"]) )
        };

        float tooltipWidth = 0;
        for (int i = 0; i < isExist.Length; i++)
        {
            if (isExist[i])
            {
                tooltipWidth += tooltipTexts[i].Item2.ToString().Length;
            }
        }
        float leftXPosition = -(tooltipWidth / 2);
        int tooltipCount = 0;
        for (int i = 0; i < isExist.Length; i++)
        {
            if (isExist[i])
            {
                float calculatedXPosition = leftXPosition + tooltipTexts[i].Item2.ToString().Length / 2.0f;
                _CharacterStatTooltipTexts[tooltipCount++].GetComponent<CharacterTooltipText>()
                    .SetCharacterTooltipText(info.statName, tooltipTexts[i].Item1, tooltipTexts[i].Item2, calculatedXPosition);
                leftXPosition += tooltipTexts[i].Item2.ToString().Length;
            }
        }
        if (tooltipWidth == 0)
        {
            float calculatedXPosition = leftXPosition + tooltipTexts[0].Item2.ToString().Length / 2.0f;
            _CharacterStatTooltipTexts[0].GetComponent<CharacterTooltipText>()
                       .SetCharacterTooltipText(info.statName, tooltipTexts[0].Item1, tooltipTexts[0].Item2, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        CloseUI();
    }
}
