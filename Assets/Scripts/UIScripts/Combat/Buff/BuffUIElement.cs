using System;
using System.Collections;
using System.Collections.Generic;
using PassiveSkill;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuffUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private static IconDatabase _iconDatabase;
    private const string ICON_DATABASE_PATH = "DataBase/IconDatabase";
    
    [SerializeField] private GameObject _buffImage;
    [SerializeField] private GameObject _buffEffect;
    [SerializeField] private GameObject _buffText;

    public IDisplayableEffect displayedEffect { get; private set; }
    private bool _isPlayer = false;
    
    private IconDatabase GetDB()
    {
        if (_iconDatabase == null)
        {
            _iconDatabase = Resources.Load<IconDatabase>(ICON_DATABASE_PATH);
        }

        if (_iconDatabase == null)
        {
            Debug.LogError($"cant found icon database from path {ICON_DATABASE_PATH}");
            throw new NullReferenceException();
        }
        
        return _iconDatabase;
    }

    public void SetBuffUIElement(IDisplayableEffect effect, bool isBuff, bool isPlayer)
    {
        displayedEffect = effect;
        _isPlayer = isPlayer;
        // buff image setting
        Sprite icon = null;
        switch (effect)
        {
            // lagecy code
            case StatusEffect sEffect:
                icon = UIManager.instance.combatUI.buffUI.GetDebuffIconSprite(sEffect.GetStatusEffectType());
                break;
            case BaseEffect:
                icon = SkillManager.instance.GetSkill(effect.GetIndex()).skillInfo.icon;
                break;
            case ItemBuff item:
                icon = GameManager.instance.itemDatabase.GetItemData(item.GetIndex()).icon;
                break;
            default:
                // 최신 코드 : icondb에서 읽어오기
                icon = GetDB().GetIcon(effect.GetIndex());
                if (icon == null)
                {
                    Debug.LogError("Unknown Effect Type : " + effect.GetType());
                }
                break;
        }
        _buffImage.GetComponent<Image>().sprite = icon;
        
        // 최신 코드 : icondb에서 읽어오기
        icon = GetDB().GetIcon(effect.GetIndex());

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
        if (duration is <= 0 or >= 100)
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
