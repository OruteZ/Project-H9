using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillListUI : UISystem
{
    [SerializeField] private GameObject _passiveSkillListContainer;
    [SerializeField] private GameObject _passiveSkillListTooltip;

    private static PassiveSkillListPool _listPool = null;

    public override void OpenUI() 
    {
        SetPassiveSkillListUI();
    }
    void Start()
    {
        ClosePassiveSkillListTooltip();
        if (_listPool == null)
        {
            _listPool = new PassiveSkillListPool();
            _listPool.Init("Prefab/Passive Skill List UI Element", _passiveSkillListContainer.transform, 0);
        }
    }

    public void SetPassiveSkillListUI()
    {
        List<int> passiveList = GameManager.instance.playerPassiveIndexList;
        if (passiveList == null) return;
        _listPool.Reset();

        for (int i = 0; i < passiveList.Count; i++)
        {
            PassiveDatabase passiveDB = SkillManager.instance.passiveDB;
            var t = _listPool.Set();
            t.Instance.GetComponent<PassiveSkillListElement>().SetPassiveSkillListElement(passiveDB.GetPassiveInfo(passiveList[i]));
            t.Instance.transform.SetAsLastSibling();
        }
    }

    public void OpenPassiveSkillListTooltip(int index, Vector3 pos)
    {
        _passiveSkillListTooltip.GetComponent<PassiveSkillListTooltip>().SetPassiveSkillListTooltip(index, pos);
    }
    public void ClosePassiveSkillListTooltip()
    {
        _passiveSkillListTooltip.GetComponent<PassiveSkillListTooltip>().CloseUI();
    }
}
