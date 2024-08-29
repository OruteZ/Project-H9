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
        SubstituteValue("conditionAmount", skillIndex);
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
        string result = "";
        while (origin.Contains(valueName))
        {
            int startIndex = origin.IndexOf(valueName) - 1;
            int endIndex = startIndex + GetSubString(origin, startIndex, origin.Length).IndexOf('>') + 1;
            string beforeString = GetSubString(origin, 0, startIndex);
            string valueKeyword = GetSubString(origin, startIndex + 1, endIndex - 1);
            string afterString = GetSubString(origin, endIndex, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);

            float value = 0;
            Skill s = SkillManager.instance.GetSkill(skillIndex);
            if (s.skillInfo.IsActive())
            {
                ActiveInfo info = SkillManager.instance.activeDB.GetActiveInfo(skillIndex);
                if (valueKeyword.Contains("effectAmount")) 
                {
                    if (valueKeyword == "effectAmount") value = info.amounts[0];
                    else 
                    {
                        value = info.amounts[valueKeyword[valueKeyword.Length - 1] - 1];
                    }
                }
                else if (valueKeyword == "damage")
                {
                    value = info.damage;
                }
                else if (valueKeyword == "cost")
                {
                    value = info.cost;
                }
                else if (valueKeyword == "range")
                {
                    value = info.range;
                }
                else if (valueKeyword == "radius")
                {
                    value = info.radius;
                }
            }
            else
            {
                PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(skillIndex);
                if (valueKeyword.Contains("conditionAmount"))
                {
                    if (valueKeyword == "conditionAmount") value = info.conditionAmount[0];
                    else
                    {
                        value = info.conditionAmount[valueKeyword[valueKeyword.Length - 1] - '0'];
                    }
                }
                else if (valueKeyword.Contains("effectAmount"))
                {
                    if (valueKeyword == "effectAmount") value = info.effectAmount[0];
                    else
                    {
                        value = info.effectAmount[valueKeyword[valueKeyword.Length - 1] - '0'];
                    }
                }
            }
            string str = value.ToString();
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, str);
            origin = afterString;
        }
        _substitutedDescription = result + origin;
    }
}
