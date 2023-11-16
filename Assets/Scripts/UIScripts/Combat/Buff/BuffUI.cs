using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUI : UISystem
{
    [SerializeField] private GameObject _BuffUI;
    [SerializeField] private GameObject _DebuffUI;
    [SerializeField] private PassiveDatabase passiveDB;

    private new void Awake()
    {
        base.Awake();
        _BuffUI.SetActive(false);
        _DebuffUI.SetActive(false);

        UIManager.instance.onTurnChanged.AddListener(SetBuffDebuffUI);
    }
    public override void OpenUI()
    {
        base.OpenUI();
        SetBuffDebuffUI();
    }

    public void SetBuffDebuffUI()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        IDisplayableEffect[] playerBuffs = player.GetDisplayableEffects();
        foreach (IDisplayableEffect effect in playerBuffs) 
        {
            Debug.Log(effect.GetName() + " / " + effect.GetDuration() + " / " + effect.GetStack() + " / " + effect.CanDisplay());
        }
        SetBuffUI();
        //SetDebuffUI();
    }
    private void SetBuffUI()
    {
        _BuffUI.SetActive(true);
        for (int i = 0; i < _BuffUI.transform.childCount; i++)
        {
            _BuffUI.transform.GetChild(i).GetComponent<BuffUIElement>().CloseUI();
        }
        int buffCount = 0;

        Player player = FieldSystem.unitSystem.GetPlayer();
        List<int> passiveList = GameManager.instance.playerPassiveIndexList;
        List<int> buffs = new List<int>();
        foreach (int index in passiveList)
        {
            PassiveSkill.Passive passive = passiveDB.GetPassive(index, player);
            Debug.Log(passive.IsEffectEnable());
            //if (passive.IsEffectEnable())
            if (true)
            {
                if (buffCount >= _BuffUI.transform.childCount)
                {
                    buffs.RemoveAt(0);
                    buffCount--;
                }
                buffs.Add(index);
                buffCount++;
            }
        }

        for (int i = 0; i < _BuffUI.transform.childCount; i++)
        {
            if (i < buffCount)
            {
                _BuffUI.transform.GetChild(i).GetComponent<BuffUIElement>().SetBuffUIElement(buffs[i], true, 0);
            }
        }
        if (buffCount == 0)
        {
            _BuffUI.SetActive(false);
        }
    }
    private void SetDebuffUI()
    {
        _DebuffUI.SetActive(true);
        for (int i = 0; i < _DebuffUI.transform.childCount; i++)
        {
            _DebuffUI.transform.GetChild(i).GetComponent<BuffUIElement>().CloseUI();
        }
        int debuffCount = 0;

        Player player = FieldSystem.unitSystem.GetPlayer();
        IDisplayableEffect[] playerBuffs = player.GetDisplayableEffects();

        List<int> passiveList = GameManager.instance.playerPassiveIndexList;
        List<int> buffs = new List<int>();
        foreach (int index in passiveList)
        {
            PassiveSkill.Passive passive = passiveDB.GetPassive(index, player);
            Debug.Log(passive.IsEffectEnable());
            //if (passive.IsEffectEnable())
            if (true)
            {
                if (debuffCount >= _DebuffUI.transform.childCount)
                {
                    buffs.RemoveAt(0);
                    debuffCount--;
                }
                buffs.Add(index);
                debuffCount++;
            }
        }

        for (int i = 0; i < _DebuffUI.transform.childCount; i++)
        {
            if (i < debuffCount)
            {
                _DebuffUI.transform.GetChild(i).GetComponent<BuffUIElement>().SetBuffUIElement(buffs[i], true, 0);
            }
        }
        if (debuffCount == 0)
        {
            _DebuffUI.SetActive(false);
        }
    }
}
