using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordScript
{
    public int index { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }

    private bool isStatusEffect;
    public KeywordScript(int idx, string str1, string str2, bool isEff)
    {
        index = idx;
        name = str1;
        description = str2;
        isStatusEffect = isEff;
    }
}
