using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionNameTooltip : UIElement
{
    [SerializeField] private GameObject _LeftTooltip;
    [SerializeField] private GameObject _LeftText;
    [SerializeField] private GameObject _RightTooltip;
    [SerializeField] private GameObject _RightText;

    private const int SIZE_CORRECTION = 50;

    private GameObject _targetButton;

    private void Update()
    {
        if (_targetButton is null) return;
        GetComponent<RectTransform>().position = _targetButton.GetComponent<RectTransform>().position;
        SizeFitter(_LeftTooltip, _LeftText);
        SizeFitter(_RightTooltip, _RightText);
    }
    private void SizeFitter(GameObject tt, GameObject text)
    {
        Vector3 size = tt.GetComponent<RectTransform>().sizeDelta;
        size.x = text.GetComponent<RectTransform>().sizeDelta.x + SIZE_CORRECTION;
        tt.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetCombatActionTooltip(GameObject btn) 
    {
        bool isRight = false;
        float angle = btn.GetComponent<RectTransform>().localEulerAngles.z;
        if (180 < angle || angle == 0) 
        {
            isRight = true;
        }

        _targetButton = btn.transform.GetChild(0).gameObject;
        GetComponent<RectTransform>().position = _targetButton.GetComponent<RectTransform>().position;
        _LeftTooltip.SetActive(!isRight);
        _RightTooltip.SetActive(isRight);

        string str = btn.GetComponent<CombatActionButtonElement>().buttonName;
        _LeftText.GetComponent<TextMeshProUGUI>().text = str;
        _RightText.GetComponent<TextMeshProUGUI>().text = str;

        OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        _targetButton = null;
    }
}
