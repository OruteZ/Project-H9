using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Skill의 속성을 저장하고 초기화하는 클래스
/// </summary>
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
        PassiveType,
        ActiveType,
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
    public int section { get; private set; }
    private int _isPassive;
    public int nameIndex { get; private set; }
    public int tooltipIndex { get; private set; }
    public Sprite icon { get; private set; }
    public int[] precedenceIndex { get; private set; }
    public List<SkillInfo> precedenceSkillInfo { get; private set; }
    public List<SkillInfo> childSkillInfo { get; private set; }

    /// <summary>
    /// SkillTable에서 한 행을 입력받아서 변수들을 초기화합니다.
    /// </summary>
    /// <param name="list"> SkillTable에서 가져온 한 행의 문자열 </param>
    public SkillInfo(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals("") || list[i].Equals("null"))
            {
                list[i] = "0";
            }
        }

        index = int.Parse(list[0]);
        section = int.Parse(list[1]);
        _isPassive = int.Parse(list[2]);
        nameIndex = int.Parse(list[3]);
        tooltipIndex = int.Parse(list[4]);
        icon = Resources.Load<Sprite>("SkillIcon/" + list[5]);
        precedenceIndex = InitIntArrayValue(list[6]);
        precedenceSkillInfo = new List<SkillInfo>();
        childSkillInfo = new List<SkillInfo>();
    }
    public void ConnectPrecedenceSkill()
    {
        if (precedenceIndex[0] == 0) return;
        for (int i = 0; i < precedenceIndex.Length; i++)
        {
            SkillInfo preSkill = SkillManager.instance.GetSkill(precedenceIndex[i]).skillInfo;
            precedenceSkillInfo.Add(preSkill);
            preSkill.AddChildInfo(this);
        }

    }
    public void AddChildInfo(SkillInfo child)
    {
        childSkillInfo.Add(child);
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
        if (_isPassive == (int)SkillActiveOrPassive.ActiveType)
        {
            return true;
        }
        return false;
    }
    public bool IsPassive() 
    {
        return !IsActive();
    }
}

/// <summary>
/// 스킬의 속성과 여러 상태 및 기능을 포함하는 클래스
/// </summary>
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

    /// <summary>
    /// isLearnable 변수를 초기화합니다.
    /// 스킬의 선행 스킬이 존재하지 않을 경우 true, 존재할 경우 false로 초기화합니다.
    /// </summary>
    private void InitIsLearnable()
    {
        isLearnable = (skillInfo.precedenceIndex[0] == 0);
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
    
    /// <summary>
    /// isLearnable 변수의 상태를 갱신합니다.
    /// 스킬을 최대치까지 배운 상태라면 false로 갱신합니다.
    /// 그렇지 않고, 선행 스킬들을 모두 배운 상태라면 true, 아니라면 false로 갱신합니다.
    /// </summary>
    /// <param name="skills"> 플레이어의 스킬 리스트 </param>
    public void UpdateIsLearnable(List<Skill> skills)
    {
        if (skillLevel > 0)
        {
            isLearnable = false;
            return;
        }

        CheckPrecedenceSkill(skills);
        isLearnable = IsLearnedAllPrecedenceSkills();
    }
    private void CheckPrecedenceSkill(IReadOnlyList<Skill> skills)
    {
        foreach (Skill t in skills)
        {
            for (int j = 0; j < skillInfo.precedenceIndex.Length; j++)
            {
                bool isPrecedenceSkill = (t.skillInfo.index == skillInfo.precedenceIndex[j]);
                if (isPrecedenceSkill)
                {
                    isLearnedPrecedeSkill[j] = t.isLearned;
                }
            }
        }
    }
    private bool IsLearnedAllPrecedenceSkills()
    {
        return isLearnedPrecedeSkill.All(t => t);
    }

    /// <summary>
    /// 해당 스킬을 배웁니다.
    /// </summary>
    public void LearnSkill()
    {
        skillLevel++;
        isLearned = true;
        isLearnable = false;
    }
}

