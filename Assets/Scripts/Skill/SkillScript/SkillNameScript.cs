using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillNameScript
{
    public int index { get; private set; }
    public string name { get; private set; }
    public SkillNameScript(int idx, string str)
    {
        index = idx;
        name = str;
    }
}
