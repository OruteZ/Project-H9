using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterTooltip : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _tooltipNameText;
    [SerializeField] private GameObject _tooltipDescText;

    public bool isMouseOver { get; private set; }
    public string nameText { get; private set; }

    private CharacterStatTextElement _textElement;

    [SerializeField] private GameObject _CharacterStatTooltipTexts;

    private const int STAT_TOOLTIP_YPOSITION_CORRECTION = 80;
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
    public void SetCharacterTooltip(CharacterStatTextElement textElement, CharacterStatUIInfo info, Vector3 position)
    {
        if (nameText == info.statName) return;

        //Tooltip setting
        OpenUI();
        nameText = info.statName;
        _textElement = textElement;
        _tooltipNameText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.statScript.GetStatScript(info.statName).name;
        _tooltipDescText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.statScript.GetStatScript(info.statName).description;
        Vector3 pos = new Vector3(position.x, position.y + 10, position.z);
        pos.y -= STAT_TOOLTIP_YPOSITION_CORRECTION;
        GetComponent<RectTransform>().position = pos;


        //Text Setting
        for (int i = 0; i < _CharacterStatTooltipTexts.transform.childCount; i++)
        {
            _CharacterStatTooltipTexts.transform.GetChild(i).GetComponent<CharacterTooltipText>().CloseUI();
        }
        if (info.statName == "Name") return;
        //bool isExistCharacterStat = (info.statValues[UIStatType.Character] != 0);
        bool isExistBaseStat = (info.statValues[UIStatType.Base] != 0);
        bool isExistCharacterStat = (info.statValues[UIStatType.Character] != 0);
        bool isExistWeaponStat = (info.statValues[UIStatType.Weapon] != 0);
        bool isExistSkillStat = (info.statValues[UIStatType.Skill] != 0);
        bool[] isExistStat =
        {
            isExistBaseStat,
            isExistCharacterStat,
            isExistSkillStat,
            isExistWeaponStat,
        };
        List<(UIStatType, string)> tooltipTexts = new List<(UIStatType, string)>()
        {
            ( UIStatType.Base,      info.GetCorrectedValue(info.statValues[UIStatType.Base]).ToString() ),
            ( UIStatType.Character, info.GetCorrectedValue(info.statValues[UIStatType.Character]).ToString() ),
            ( UIStatType.Skill,     info.GetCorrectedValue(info.statValues[UIStatType.Skill]).ToString() ),
            ( UIStatType.Weapon,    info.GetCorrectedValue(info.statValues[UIStatType.Weapon]).ToString() )
        };
        (UIStatType, string) plusSign = (UIStatType.Sign, "+");

        List<(UIStatType, string)> existText = new List<(UIStatType, string)>();
        for (int i = 0; i < isExistStat.Length; i++)
        {
            //if (isExistStat[i])
            if (isExistStat[i]) 
            {
                if (existText.Count != 0 && existText.Count % 2 == 1) 
                {
                    existText.Add(plusSign);
                }
                existText.Add(tooltipTexts[i]);
            } 
        }
        if (existText.Count == 0) 
        {
            existText.Add(tooltipTexts[0]);
        }

        for (int i = 0; i < _CharacterStatTooltipTexts.transform.childCount; i++)
        {
            GameObject textObject = _CharacterStatTooltipTexts.transform.GetChild(i).gameObject;
            if (i < existText.Count)
            {
                //공백 추가 (UI 자동 정렬 시 텍스트의 뒤에 붙은 공백은 인식을 못해서 앞에다가 붙임.)
                if (i != 0)
                {
                    (UIStatType, string) tmpItem = existText[i];
                    tmpItem.Item2 = ' ' + tmpItem.Item2;
                    existText[i] = tmpItem;
                }
                textObject.SetActive(true);
                textObject.GetComponent<CharacterTooltipText>().SetCharacterTooltipText(info.statName, existText[i].Item1, existText[i].Item2);
            }
            else 
            {
                textObject.SetActive(false);
            }
        }
        _CharacterStatTooltipTexts.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        _CharacterStatTooltipTexts.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
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
