using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public int index {get; set;}
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
        for (int i = 0; i < _list.Count; i++) 
        {
            if (_list[i].Equals("")) 
            {
                _list[i] = "0";
            }
        }

        index = int.Parse(_list[0]);
        icon = _list[1];
        name = _list[2];
        description = _list[3];
        category = int.Parse(_list[4]);
        aop = int.Parse(_list[5]);
        preced = InitIntArrayValue(_list[6]);
        repeat = int.Parse(_list[7]);
        stat = InitIntArrayValue(_list[8]);
        activeType = int.Parse(_list[9]);
        upgSkill = int.Parse(_list[10]);
        amount = InitIntArrayValue(_list[11]);
        range = int.Parse(_list[12]);
        width = int.Parse(_list[13]);
    }
    private int[] InitIntArrayValue(string _string) 
    {
        if (_string.Length == 0) return new int[] { 0 };

        string[] splitString = _string.Split('$');

        int[] result = new int[splitString.Length];

        for (int i = 0; i < splitString.Length; i++) 
        {
            result[i] = int.Parse(splitString[i]);
        }

        return result;
    } 
}

public class SkillManager : MonoBehaviour
{
    private List<List<string>> skillTable;
    private List<Skill> skills;

    private void Awake()
    {
        SetSkills();

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SetSkills()
    {
        skillTable = SkillRead.Read("SkillTable");
        if (skillTable == null)
        {
            Debug.Log("skill table을 읽어오지 못했습니다.");
        }

        skills = new List<Skill>();
        for (int i = 0; i < skillTable.Count; i++)
        {
            Skill skill = new Skill(skillTable[i]);
            skills.Add(skill);
        }


        for (int i = 0; i < skills.Count; i++)
        {
            Debug.Log(skills[i].index);

        }
    }
}
