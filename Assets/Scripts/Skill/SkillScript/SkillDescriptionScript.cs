using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillDescriptionScript
{
    public int index { get; private set; }
    private string _description;
    private List<int> _keywordIndex;

    public SkillDescriptionScript(int idx, string dsc)
    {
        index = idx;
        _description = dsc;
        _keywordIndex = new List<int>();
    }
    public string GetDescription(int skiilIndex)
    {
        string result;
        result = SubstituteDescriptionValues(skiilIndex);
        result = SubstituteKeyword(result);
        return result;
    }
    private string SubstituteDescriptionValues(int skillIndex)  //대입할 수가 설명 맨 앞에 오는 경우 오류 가능성 높음.
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
                string amountText;
                if (SkillManager.instance.GetSkill(skillIndex).skillInfo.IsPassive())
                {
                    //현재 effectAmount가 배열 형식이 아닌 1개의 변수만을 지니고 있는데,
                    //기획 측의 말에 따르면 여러 변수가 저장될 수도 있다고 해서 만약 구조가 변경되면 이 부분도 수정해야 함.
                    PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(skillIndex);
                    amountText = info.effectAmount.ToString();
                }
                else
                {
                    ActiveInfo info = SkillManager.instance.activeDB.GetActiveInfo(skillIndex);
                    amountText = info.amounts[0].ToString();
                }
                string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
                result += string.Format("<color=#{0}>{1}</color>", highlightColor, amountText);
                //result += "<color=#" + HIGHLIGHT_COLOR + ">" + amountText + "</color>";
            }
            isSubstitutableValue = !isSubstitutableValue;
        }
        return result;
    }
    private string SubstituteKeyword(string valueSubstitutedString)
    {
        string vss = valueSubstitutedString;
        return vss;  //test
        string[] split = { "<tooltip:", "</tooltip>" };
        string result = "";
        while (vss.Contains(split[0])) 
        {
            string beforeString = vss.Substring(0, vss.IndexOf(split[0]));
            string middleString = vss.Substring(vss.IndexOf(split[0]) + split.Length + 4, vss.IndexOf(split[1]) - 1);
            string afterString = vss.Substring(vss.IndexOf(split[1]) + split[1].Length, vss.Length - 1);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, middleString);
            vss = afterString;
        }
        return result;
    }
}
