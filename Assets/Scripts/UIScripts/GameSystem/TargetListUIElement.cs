using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TargetListUIElement : UIElement
{
    [SerializeField] private GameObject _targetPortraitImage;
    [SerializeField] private GameObject _targetPortraitEffect;
    [SerializeField] private GameObject _targetNameText;
    [SerializeField] private GameObject _targetNicknameText;

    [SerializeField] private string _targetPortrait;
    [SerializeField] private int _targetNameIndex;
    [SerializeField] private int _targetNicknameIndex;
    [SerializeField] private int[] _targetQuestList;

    private TargetListUI _targetListUI;

    public void SetTargetListUIElement(TargetListUI ui) 
    {
        _targetListUI = ui;
        if (_targetPortrait != "")
        {
            Texture2D enemyTexture = Resources.Load("UnitCapture/" + _targetPortrait) as Texture2D;
            Sprite enemySpr = Sprite.Create(enemyTexture, new Rect(0, 0, enemyTexture.width, enemyTexture.height), new Vector2(0.5f, 0.5f));
            _targetPortraitImage.GetComponent<Image>().sprite = enemySpr;
        }

        _targetNameText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[_targetNameIndex];
        _targetNicknameText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[_targetNicknameIndex];

        Color textColor = Color.white;
        if (EventSystem.current.currentSelectedGameObject == this.gameObject) textColor = Color.black;
        _targetNameText.GetComponent<TextMeshProUGUI>().color = textColor;
        _targetNicknameText.GetComponent<TextMeshProUGUI>().color = textColor;

        if (_targetQuestList.Length > 0)
        {
            foreach (var q in GameManager.instance.Quests)
            {
                if (q.Index == _targetQuestList[_targetQuestList.Length - 1] && q.IsCleared)
                {
                    _targetPortraitEffect.SetActive(true);
                    return;
                }
            }
        }
        _targetPortraitEffect.SetActive(false);
    }
    public int[] GetTargetQuestList => _targetQuestList;
    public void OnClickTargetListUI() 
    {
        _targetListUI.OpenTargetQuestList(_targetQuestList);
    }
}
