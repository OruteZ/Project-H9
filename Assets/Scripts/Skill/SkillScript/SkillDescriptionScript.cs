using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillDescriptionScript
{
    public int index { get; private set; }
    public string description { get; private set; }
    public List<int> keywordIndex { get; private set; }

    private static string substitutedDescription ="";
    public SkillDescriptionScript(int idx, string str)
    {
        index = idx;
        description = str;
        keywordIndex = new List<int>();
    }
    public string GetDescription(int skiilIndex)
    {
        substitutedDescription = description;
        SubstituteKeyword();
        SubstituteDescriptionValues(skiilIndex);
        if (substitutedDescription[0] == '\"') 
        {
            substitutedDescription = substitutedDescription.Substring(1, substitutedDescription.Length - 2);

        }
        if (keywordIndex.Count != 0)
        {
            UIManager.instance.skillUI.SetKeywordTooltipContents(keywordIndex);
        }
        return substitutedDescription;
    }
    private void SubstituteDescriptionValues(int skillIndex)  //대입할 수가 설명 맨 앞에 오는 경우 오류 가능성 높음.
    {
        string result = "";
        char[] splitChar = { '{', '}' };
        string[] splitString = substitutedDescription.Split(splitChar);
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
                    amountText = info.amounts[int.Parse(str)].ToString();
                }
                string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
                result += string.Format("<color=#{0}>{1}</color>", highlightColor, amountText);
            }
            isSubstitutableValue = !isSubstitutableValue;
        }
        substitutedDescription = result;
    }
    private void SubstituteKeyword()
    {
        string origin = substitutedDescription;
        string[] split = { "<keyword:", ">" };
        string result = "";
        keywordIndex.Clear();
        while (origin.Contains(split[0]))
        {
            string beforeString = GetSubString(origin, 0, origin.IndexOf(split[0]));
            string middleString = GetSubString(origin, origin.IndexOf(split[0]) + split[0].Length, origin.IndexOf(split[1]));
            string afterString = GetSubString(origin, origin.IndexOf(split[1]) + split[1].Length, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
            keywordIndex.Add(int.Parse(middleString));
            string keyword = SkillManager.instance.GetSkillKeyword(int.Parse(middleString)).name;
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, keyword);
            origin = afterString;
        }
        substitutedDescription = result + origin;
    }
    private string GetSubString(string origin, int startIndex, int endIndex) 
    {
        int length = endIndex - startIndex;
        return origin.Substring(startIndex, length);
    }
}
