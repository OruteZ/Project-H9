using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordScript
{
    public int index { get; private set; }
    public string name { get; private set; }
    public string Ename { get; private set; }
    public string description { get; private set; }
    public bool isStatusEffect { get; private set; }

    public List<int> keywordIndex { get; private set; }

    private static string _substitutedDescription = "";
    public KeywordScript(int idx, string str1, string str2, string str3, bool isEff)
    {
        index = idx;
        name = str1;
        Ename = str2;
        description = str3;
        isStatusEffect = isEff;

        keywordIndex = new List<int>();
    }
    public string GetDescription()
    {
        _substitutedDescription = description;
        SubstituteKeyword();
        if (_substitutedDescription[0] == '\"')
        {
            _substitutedDescription = _substitutedDescription.Substring(1, _substitutedDescription.Length - 2);

        }
        if (keywordIndex.Count != 0)
        {
            UIManager.instance.skillUI.SetKeywordTooltipContents(keywordIndex);
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
            keywordIndex.Add(int.Parse(middleString));
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
}
