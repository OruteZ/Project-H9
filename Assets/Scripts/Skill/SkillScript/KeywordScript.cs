using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordScript
{
    public int index { get; private set; }
    public string name { get; private set; }
    public string Ename { get; private set; }
    public string description { get; private set; }

    private bool isStatusEffect;
    public KeywordScript(int idx, string str1, string str2, string str3, bool isEff)
    {
        index = idx;
        name = str1;
        Ename = str2;
        description = str3;
        isStatusEffect = isEff;
    }
}
