using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterTooltip : UIElement
{
    // Start is called before the first frame update
    void Start()
    {
        CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    public void SetCharacterTooltip(string name, float yPosition) 
    {
        OpenUI();
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.y = yPosition;
        GetComponent<RectTransform>().position = pos;
    }
}
