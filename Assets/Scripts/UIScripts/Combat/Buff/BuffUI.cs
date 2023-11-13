using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUI : UISystem
{
    [SerializeField] private GameObject _BuffUI;
    [SerializeField] private PassiveDatabase passiveDB;

    private new void Awake()
    {
        base.Awake();
        _BuffUI.SetActive(false);

        UIManager.instance.onTurnChanged.AddListener(SetBuffUI);
    }
    public override void OpenUI()
    {
        base.OpenUI();
        SetBuffUI();
    }

    public void SetBuffUI()
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
    }
}
