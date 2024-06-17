using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuffUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private GameObject _buffImage;
    [SerializeField] private GameObject _buffEffect;
    [SerializeField] private GameObject _buffText;

    public IDisplayableEffect displayedEffect { get; private set; }
    private bool _isPlayer = false;

    public void SetBuffUIElement(IDisplayableEffect effect, bool isBuff, bool isPlayer)
    {
        displayedEffect = effect;
        _isPlayer = isPlayer;
        // buff image setting
        Sprite icon = null;
        if (effect is StatusEffect sEffect)
        {
            icon = UIManager.instance.combatUI.buffUI.GetDebuffIconSprite(sEffect.GetStatusEffectType());
        }
        else if (effect is PassiveSkill.BaseEffect)
        {
            icon = SkillManager.instance.GetSkill(effect.GetIndex()).skillInfo.icon;
        }
        else if (effect is ItemBuff item)
        {
            icon = GameManager.instance.itemDatabase.GetItemData(item.GetIndex()).icon;
        }
        _buffImage.GetComponent<Image>().sprite = icon;

        //Outline Effect Setting
        if (isBuff)
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.BuffColor;
        }
        else
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.DebuffColor;
        }

        //Text Setting
        int duration = effect.GetDuration();
        if (effect is Bleeding) 
        {
            duration = effect.GetStack();
        }

        string durationText = duration.ToString();
        if (duration <= 0 || duration >= 100)
        {
            durationText = "";
        }
        _buffText.GetComponent<TextMeshProUGUI>().text = durationText;
        OpenUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (displayedEffect is not null && _isPlayer)
        {
            UIManager.instance.combatUI.buffUI.ShowBuffITooltip(this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (displayedEffect is not null && _isPlayer)
        {
            UIManager.instance.combatUI.buffUI.HideBuffUITooltip();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (displayedEffect is not null && _isPlayer)
        {
            UIManager.instance.combatUI.buffUI.ShowBuffITooltip(this.gameObject);
        }
    }
}
