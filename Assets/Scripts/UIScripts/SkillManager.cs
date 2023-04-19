using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Skill
{
    private int index {get; set;}
    private string icon { get; set; }
    private string name { get; set; }
    private string description { get; set; }
    private int category { get; set; }
    private int aop { get; set; }   //aop??
    private int[] preced { get; set; }
    private int repeat { get; set; }
    private int[] stat { get; set; }
    private int activeType { get; set; }
    private int upgSkill { get; set; }
    private int[] amount { get; set; }
    private int range { get; set; }
    private int width { get; set; }

    public Skill(List<string> _list) 
    {
        InitValues(_list);
    }

    private void InitValues(List<string> _list) 
    {
        index = int.Parse(_list[0]);
        icon = _list[1];
        name = _list[2];
        description = _list[3];
        category = int.Parse(_list[0]);
        aop = int.Parse(_list[0]);
        preced = InitIntArrayValue(_list[0]);
        repeat = int.Parse(_list[0]);
        stat = InitIntArrayValue(_list[0]);
        activeType = int.Parse(_list[0]);
        upgSkill = int.Parse(_list[0]);
        amount = InitIntArrayValue(_list[0]);
        range = int.Parse(_list[0]);
        width = int.Parse(_list[0]);
    }
    private int[] InitIntArrayValue(string _string) 
    {
        int[] result = { 0 };

        return result;
    } 
}

public class SkillManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
