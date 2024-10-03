using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : UISystem
{
    [SerializeField] private GameObject _BuffDebuffWindow;
    [SerializeField] private GameObject _BuffUI;
    [SerializeField] private GameObject _DebuffUI;
    [SerializeField] private GameObject _buffTooltipWindow;
    private IDisplayableEffect _currentTooltipEffect;
    [SerializeField] private PassiveDatabase passiveDB;

    private List<IDisplayableEffect> _currentBuffs = new List<IDisplayableEffect>();
    private List<IDisplayableEffect> _currentDebuffs = new List<IDisplayableEffect>();
    private List<StatusEffectType> _debuffSortPriority = new List<StatusEffectType>()
    {
        StatusEffectType.Stun,
        StatusEffectType.UnArmed,
        StatusEffectType.Recoil,
        StatusEffectType.Taunted,
        StatusEffectType.Concussion,
        StatusEffectType.Fracture,
        StatusEffectType.Blind,
        StatusEffectType.Bleeding,
        StatusEffectType.Burning,
        //StatusEffectType.Rooted
    };

    private void Awake()
    {
        _BuffUI.SetActive(false);
        _DebuffUI.SetActive(false);

        UIManager.instance.onPlayerStatChanged.AddListener(SetBuffDebuffUI);
        UIManager.instance.onActionChanged.AddListener(SetBuffDebuffUI);
        PlayerEvents.OnWeaponChanged.AddListener((w) => { SetBuffDebuffUI(); });
    }

    private void StageAwake()
    {
        FieldSystem.turnSystem.onTurnChanged.AddListener(SetBuffDebuffUI);
    }
    
    public override void OpenUI()
    {
        base.OpenUI();
        SetBuffDebuffUI();
    }

    public void SetBuffDebuffUI()
    {
        if (!GameManager.instance.CompareState(GameState.COMBAT)) return; 
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null) return;
        IDisplayableEffect[] playerBuffs = player.GetDisplayableEffects();
        _currentBuffs.Clear();
        _currentDebuffs.Clear();
        foreach (IDisplayableEffect effect in playerBuffs)
        {
            if (effect is StatusEffect)
            {
                _currentDebuffs.Add(effect);
                
            }
            else
            {
                _currentBuffs.Add(effect);
            }
        }
        SetBuffUI(_BuffUI, _currentBuffs, true);
        SortDebuff();
        SetBuffUI(_DebuffUI, _currentDebuffs, false);
        _BuffDebuffWindow.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        _BuffDebuffWindow.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }
    private void SortDebuff()
    {
        List<IDisplayableEffect> sortedList = new List<IDisplayableEffect>();
        for (int i = 0; i < _debuffSortPriority.Count; i++)
        {
            for (int j = _currentDebuffs.Count - 1; j >= 0; j--)
            {
                if (((StatusEffect)_currentDebuffs[j]).GetStatusEffectType() == _debuffSortPriority[i]) 
                {
                    sortedList.Add(_currentDebuffs[j]);
                    _currentDebuffs.RemoveAt(j);
                }
            }
        }
        for (int j = 0; j < _currentDebuffs.Count; j++)
        {
            sortedList.Add(_currentDebuffs[j]);
        }
        _currentDebuffs = sortedList;
    }
    private void SetBuffUI(GameObject UI, List<IDisplayableEffect> currentState, bool isBuff)
    {
        UI.SetActive(true);
        for (int i = 0; i < UI.transform.childCount; i++)
        {
            UI.transform.GetChild(i).GetComponent<BuffUIElement>().CloseUI();
        }
        int buffCount = 0;

        bool isExistTooltipEffect = false;
        foreach (IDisplayableEffect effect in currentState)
        {
            if (effect.CanDisplay())
            {
                UI.transform.GetChild(buffCount++).GetComponent<BuffUIElement>().SetBuffUIElement(effect, isBuff, true);
                if (effect == _currentTooltipEffect)
                {
                    isExistTooltipEffect = true;
                }
            }
        }
        if (!isExistTooltipEffect) HideBuffUITooltip();

        if (buffCount == 0)
        {
            UI.SetActive(false);
        }
    }

    public void ShowBuffITooltip(GameObject icon)
    {
        Vector3 pos = icon.GetComponent<RectTransform>().position;
        IDisplayableEffect effect = icon.GetComponent<BuffUIElement>().displayedEffect;
        if (effect == null) return;
        _currentTooltipEffect = effect;

        RectTransform rt = _BuffDebuffWindow.GetComponent<RectTransform>();
        //Debug.Log(rt.position.y + " / " + rt.sizeDelta.y);
        pos.y = rt.position.y + (rt.sizeDelta.y + 5) * UIManager.instance.GetCanvasScale();
        _buffTooltipWindow.GetComponent<BuffTooltip>().SetBuffTooltip(effect, pos);
    }
    public void HideBuffUITooltip()
    {
        _buffTooltipWindow.GetComponent<BuffTooltip>().CloseUI();
    }
    public GameObject GetBuffWindow() 
    {
        return _BuffDebuffWindow;
    }

    public Sprite GetDebuffIconSprite(StatusEffectType effType) 
    {
        if ((int)effType >= (int)StatusEffectType.Burning && (int)effType <= (int)StatusEffectType.Rooted) 
        {
            //Debug.Log(effType);
            return UIManager.instance.iconDB.GetIconInfo(effType.ToString());
        }
        Debug.LogError("���� �̻� �������� ã�� �� �����ϴ�.");
        return null;
    }
}
