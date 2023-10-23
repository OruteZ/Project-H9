using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKeywordScript
{
    public int index { get; private set; }
    public string keyword { get; private set; }
    public SkillKeywordScript(int idx, string str)
    {
        index = idx;
        keyword = str;
    }
}
