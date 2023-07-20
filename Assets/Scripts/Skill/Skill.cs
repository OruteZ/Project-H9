using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillInfo
{
    enum SkillCategory
    {
        Null,
        Character,
        Revoler,
        Repeater,
        Shotgun
    }
    enum SkillActiveOrPassive
    {
        Null,
        Active,
        Passive
    }
    enum Stat
    {
        Null,
        HP,
        Concentration,
        Speed,
        FinalHitRate,
        Sight_Distance,
        Final_Mobility,
        FinalDamagePercent,
        FinalDamageInteger,
        HPRecovery,
        ActiveSkillDamageInteger,
        SkillDistance,
        SkillUseCount,
        UpHitRate,
        UpDistance,
        SkillDamageInteger
    }
    enum SkillActiveType
    {
        Null,
        Damage,
        Explode,
        Heal
    }

    public int index { get; private set; }
    public int iconNumber { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public int category { get; private set; }

    private int activeOrPassive;
    public int[] precedenceIndex { get; private set; }
    public int repeatCount { get; private set; }
    public int[] stat { get; private set; }
    public int activeType { get; private set; }
    public int upgradeSkill { get; private set; }
    public int[] amount { get; private set; }
    public int range { get; private set; }
    public int width { get; private set; }

    public SkillInfo(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(""))
            {
                list[i] = "0";
            }
        }

        index = int.Parse(list[0]);
        iconNumber = int.Parse(list[1]);
        name = list[2];
        description = list[3];
        category = int.Parse(list[4]);
        activeOrPassive = int.Parse(list[5]);
        precedenceIndex = InitIntArrayValue(list[6]);
        repeatCount = int.Parse(list[7]);
        if (repeatCount == 0) repeatCount = 1;  //table issue
        stat = InitIntArrayValue(list[8]);
        activeType = int.Parse(list[9]);
        upgradeSkill = int.Parse(list[10]);
        amount = InitIntArrayValue(list[11]);
        range = int.Parse(list[12]);
        width = int.Parse(list[13]);
    }
    private int[] InitIntArrayValue(string str)
    {
        const char SPLIT_CHAR = '$';
        if (str.Equals("0")) return new int[] { 0 };

        string[] splitString = str.Split(SPLIT_CHAR);

        int[] result = new int[splitString.Length];

        for (int i = 0; i < splitString.Length; i++)
        {
            result[i] = int.Parse(splitString[i]);
        }

        return result;
    }

    public bool IsActive()
    {
        if (activeOrPassive.Equals(SkillActiveOrPassive.Active))
        {
            return true;
        }
        return false;
    }
}

public class Skill
{
    public SkillInfo skillInfo { get; private set; }

    public bool isLearned { get; private set; }
    public bool isLearnable { get; private set; }
    public int skillLevel { get; private set; }
    public bool[] isLearnedPrecedeSkill { get; private set; }


    public Skill(SkillInfo info)
    {
        skillInfo = info;

        isLearned = false;
        InitIsLearnable();
        skillLevel = 0;
        InitIsLearnedPrecedeSkill();
    }

    private void InitIsLearnable()
    {
        if (skillInfo.precedenceIndex[0] == 0) isLearnable = true;
        else isLearnable = false;
    }
    private void InitIsLearnedPrecedeSkill()
    {
        isLearnedPrecedeSkill = new bool[skillInfo.precedenceIndex.Length];
        for (int i = 0; i < isLearnedPrecedeSkill.Length; i++)
        {
            isLearnedPrecedeSkill[i] = false;
        }
        if (skillInfo.precedenceIndex[0] == 0)
        {
            isLearnedPrecedeSkill[0] = true;
        }
    }
    public void UpdateIsLearnable(List<Skill> skills)
    {

        if (skillLevel >= skillInfo.repeatCount)
        {
            isLearnable = false;
            return;
        }

        CheckPrecedenceSkill(skills);
        if (IsLearnedAllPrecedenceSkills()) isLearnable = true;
    }
    private void CheckPrecedenceSkill(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            for (int j = 0; j < skillInfo.precedenceIndex.Length; j++)
            {
                bool isPrecedenceSkill = (skills[i].skillInfo.index == skillInfo.precedenceIndex[j]);
                if (isPrecedenceSkill)
                {
                    isLearnedPrecedeSkill[j] = skills[i].isLearned;
                }
            }
        }
    }
    private bool IsLearnedAllPrecedenceSkills()
    {
        for (int i = 0; i < isLearnedPrecedeSkill.Length; i++)
        {
            if (!isLearnedPrecedeSkill[i]) return false;
        }
        return true;
    }

    public void LearnSkill()
    {
        if (skillLevel >= skillInfo.repeatCount) { Debug.Log("스킬 레벨 비정상적 상승"); return; }
        skillLevel++;
        isLearned = true;
        if (skillLevel >= skillInfo.repeatCount)
        {
            isLearnable = false;
        }
    }
}

