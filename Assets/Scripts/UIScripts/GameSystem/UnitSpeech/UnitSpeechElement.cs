using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitSpeechElement : UIElement
{
    public Unit unit { get; set; }
    private Vector3 _prevUnitPos;
    private Vector3 _prevUnitUIPos;

    [SerializeField] private float UNIT_TEXT_UI_Y_POSITION_CORRECTION;
    public void SetUnitSpeech(Unit u, List<string> str)
    {
        if (str.Count < 1)
        {
            DeleteUnitSpeech();
            return;
        }
        _prevUnitPos = Vector3.zero;
        unit = u;
        StartCoroutine(DisplayUnitSpeech(str));
    }
    IEnumerator DisplayUnitSpeech(List<string> str)
    {
        for (int i = 0; i < str.Count; i++)
        {
            GetComponent<TextMeshProUGUI>().text = str[i];
            yield return new WaitForSeconds(1.0f + str[i].Length / 5.0f);
        }
        DeleteUnitSpeech();
        yield break;
    }
    private void DeleteUnitSpeech()
    {
        unit = null;
        GetComponent<RectTransform>().position = Vector3.zero;
        UIManager.instance.gameSystemUI.speechUI.ClearUnitSpeechText(gameObject);
    }
    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        //UI Position Setting
        if (unit == null)
        {
            DeleteUnitSpeech();
            return;
        }
        _prevUnitPos = unit.transform.position;
        GetComponent<TextMeshProUGUI>().enabled = unit.meshVisible;
        Vector3 unitPositionHeightCorrection = _prevUnitPos;
        unitPositionHeightCorrection.y += 1.8f;
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(unitPositionHeightCorrection);
        if (uiPosition != _prevUnitUIPos)
        {
            uiPosition.y += UNIT_TEXT_UI_Y_POSITION_CORRECTION;
            if(unit is Enemy) uiPosition.y += UNIT_TEXT_UI_Y_POSITION_CORRECTION;
            GetComponent<RectTransform>().position = uiPosition;

            _prevUnitUIPos = uiPosition;
        }
    }
}
