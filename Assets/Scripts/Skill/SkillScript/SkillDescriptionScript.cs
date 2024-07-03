using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SkillDescriptionScript
{
    public int index { get; private set; }
    public string description { get; private set; }
    public List<int> keywordIndex { get; private set; }

    private static string _substitutedDescription = "";
    public SkillDescriptionScript(int idx, string str)
    {
        index = idx;
        description = str;
        keywordIndex = new List<int>();
    }
    public string GetDescription(int skillIndex, out List<int> keywords)
    {
        _substitutedDescription = description;
        if (_substitutedDescription.Length == 0) 
        {
            keywords = null;
            return "NULL Skill Localization table error";
        }
        SubstituteKeyword();
        SubstituteValue("effectAmount", skillIndex);
        SubstituteValue("damage", skillIndex);
        SubstituteValue("cost", skillIndex);
        SubstituteValue("range", skillIndex);
        SubstituteValue("radius", skillIndex);
        //SubstituteDescriptionValues(skiilIndex);
        if (_substitutedDescription[0] == '\"') 
        {
            _substitutedDescription = _substitutedDescription.Substring(1, _substitutedDescription.Length - 2);

        }
        if (keywordIndex.Count != 0)
        {
            keywords = keywordIndex.Distinct().ToList();    //중복 제거
        }
        else
        {
            keywords = null;
        }

        return _substitutedDescription;
    }
    private void SubstituteKeyword()
    {
        string origin = _substitutedDescription;
        string[] split = { "<keyword:", ">" };
        string result = "";
        keywordIndex.Clear();
        while (origin.Contains(split[0]))
        {
            int startIndex = origin.IndexOf(split[0]);
            int endIndex = startIndex + GetSubString(origin, origin.IndexOf(split[0]), origin.Length).IndexOf(split[1]);
            string beforeString = GetSubString(origin, 0, startIndex);
            string middleString = GetSubString(origin, startIndex + split[0].Length, endIndex);
            string afterString = GetSubString(origin, endIndex + split[1].Length, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
            int addedKeywordIndex = int.Parse(middleString);
            keywordIndex.Add(addedKeywordIndex);
            if(addedKeywordIndex == 1) keywordIndex.Add(2);
            string keyword = SkillManager.instance.GetSkillKeyword(int.Parse(middleString)).name;
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, keyword);
            origin = afterString;
        }
        _substitutedDescription = result + origin;
    }
    private string GetSubString(string origin, int startIndex, int endIndex) 
    {
        int length = endIndex - startIndex;
        return origin.Substring(startIndex, length);
    }


    private void SubstituteValue(string valueName, int skillIndex)
    {
        string origin = _substitutedDescription;
        string split = "<" + valueName + ">";
        string result = "";
        while (origin.Contains(split))
        {
            int startIndex = origin.IndexOf(split);
            int endIndex = startIndex + split.Length;
            string beforeString = GetSubString(origin, 0, startIndex);
            string afterString = GetSubString(origin, endIndex, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);

            float value = 0;
            Skill s = SkillManager.instance.GetSkill(skillIndex);
            if (s.skillInfo.IsActive())
            {
                ActiveInfo info = SkillManager.instance.activeDB.GetActiveInfo(skillIndex);
                if (valueName == "effectAmount") 
                {
                    value = info.amounts[0];
                }
                else if (valueName == "damage")
                {
                    value = info.damage;
                }
                else if (valueName == "cost")
                {
                    value = info.cost;
                }
                else if (valueName == "range")
                {
                    value = info.range;
                }
                else if (valueName == "radius")
                {
                    value = info.radius;
                }
            }
            else
            {
                PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(skillIndex);
                if (valueName == "effectAmount")
                {
                    value = info.effectAmount[0];
                }
            }
            string str = value.ToString();
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, str);
            origin = afterString;
        }
        _substitutedDescription = result + origin;
    }

    //private void SubstituteDescriptionValues(int skillIndex)  //대입할 수가 설명 맨 앞에 오는 경우 오류 가능성 높음.
    //{
    //    string result = "";
    //    char[] splitChar = { '{', '}' };
    //    string[] splitString = _substitutedDescription.Split(splitChar);
    //    bool isSubstitutableValue = false;
    //    foreach (string str in splitString)
    //    {
    //        if (!isSubstitutableValue)
    //        {
    //            result += str;
    //        }
    //        else
    //        {
    //            string amountText;
    //            if (SkillManager.instance.GetSkill(skillIndex).skillInfo.IsPassive())
    //            {
    //                //현재 effectAmount가 배열 형식이 아닌 1개의 변수만을 지니고 있는데,
    //                //기획 측의 말에 따르면 여러 변수가 저장될 수도 있다고 해서 만약 구조가 변경되면 이 부분도 수정해야 함.
    //                PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(skillIndex);
    //                amountText = info.effectAmount.ToString();
    //            }
    //            else
    //            {
    //                ActiveInfo info = SkillManager.instance.activeDB.GetActiveInfo(skillIndex);
    //                amountText = info.amounts[int.Parse(str)].ToString();
    //            }
    //            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);
    //            result += string.Format("<color=#{0}>{1}</color>", highlightColor, amountText);
    //        }
    //        isSubstitutableValue = !isSubstitutableValue;
    //    }
    //    _substitutedDescription = result;
    //}
}
