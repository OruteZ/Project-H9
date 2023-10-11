using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillDescriptionScript
{
    public int index { get; private set; }
    private string _description;
    private bool _isSubstituted;
    public SkillDescriptionScript(int idx, string dsc)
    {
        index = idx;
        _description = dsc;
        _isSubstituted = false;
    }
    public string GetDescription(int skiilIndex)
    {
        if (!_isSubstituted)
        {
        }
        if (!_isSubstituted)
        {
            Debug.LogError("툴팁 변수 대입에 실패했습니다.");
        }
        return SubstituteDescriptionValues(skiilIndex);
    }
    private string SubstituteDescriptionValues(int skillIndex)
    {
        string result = "";
        char[] splitChar = { '{', '}' };
        string[] splitString = _description.Split(splitChar);
        bool isSubstitutableValue = false;
        foreach (string str in splitString)
        {
            if (!isSubstitutableValue)
            {
                result += str;
            }
            else
            {
                Debug.Log(skillIndex + " / " + SkillManager.instance.GetSkill(skillIndex).skillInfo.isPassive);
                if (SkillManager.instance.GetSkill(skillIndex).skillInfo.IsPassive())
                {
                    PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(skillIndex);
                    result += info.effectAmount.ToString();
                    //현재 effectAmount가 배열 형식이 아닌 1개의 변수만을 지니고 있는데,
                    //기획 측의 말에 따르면 여러 변수가 저장될 수도 있다고 해서 만약 구조가 변경되면 이 부분도 수정해야 함.
                }
                else
                {
                    ActiveInfo info = SkillManager.instance.activeDB.GetActiveInfo(skillIndex);
                    result += info.amounts[0].ToString();
                }
                Debug.Log(result);
            }
            isSubstitutableValue = !isSubstitutableValue;
        }
        return result;
    }
}
