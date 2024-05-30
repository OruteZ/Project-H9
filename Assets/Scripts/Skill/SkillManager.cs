using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임에서 사용되는 스킬의 획득, 사용 등의 기능을 관리하는 클래스
/// </summary>
public class SkillManager : Generic.Singleton<SkillManager>
{
#if UNITY_EDITOR
    private const int REQUIRED_SKILL_POINT = 0;
    private const int INITIAL_SKILL_POINT = 10;
#else
    private const int REQUIRED_SKILL_POINT = 1;
    private const int INITIAL_SKILL_POINT = 0;
#endif
    public PassiveDatabase passiveDB;
    public ActiveDatabase activeDB;

    private ScriptLanguage _language = ScriptLanguage.Korean;
    private List<Skill> _skills;
    private List<SkillNameScript> _skillNameScripts;
    private List<SkillDescriptionScript> _skillDescriptionScripts;
    private List<KeywordScript> _skillKeywordScripts;

    private int _sp = 0;
    private int _skillPoint 
    {
        get 
        {
            return _sp;
        } 
        set 
        {
            _sp = value;
            UIManager.instance.gameSystemUI.ChangeSkillButtonRedDotText(value);
        }
    }

    private new void Awake()
    {
        base.Awake();

        InitSkills();
        InitSkillScripts();
        _skillPoint = INITIAL_SKILL_POINT;
    }

    private void InitSkills()
    {
        List<List<string>> skillTable = FileRead.Read("SkillTable", out var columnInfo);
        if (skillTable == null) 
        {
            Debug.Log("skill table을 읽어오지 못했습니다.");
            return;
        }

        List<SkillInfo> _skillInformations = new List<SkillInfo>();
        for (int i = 0; i < skillTable.Count; i++)
        {
            SkillInfo skillInfo = new SkillInfo(skillTable[i]);
            _skillInformations.Add(skillInfo);
        }

        _skills = new List<Skill>();
        for (int i = 0; i < _skillInformations.Count; i++)
        {
            Skill skill = new Skill(_skillInformations[i]);
            _skills.Add(skill);
        }
        foreach (SkillInfo info in _skillInformations)
        {
            info.ConnectPrecedenceSkill();
        }
    }
    private void InitSkillScripts()
    {
        List<List<string>> skillNameTable = FileRead.Read("SkillNameScriptTable", out var nameColumnInfo);
        List<List<string>> skillDescriptionTable = FileRead.Read("SkillTooltipScriptTable", out var descColumnInfo);
        List<List<string>> skillKeywordTable = FileRead.Read("KeywordTable", out var keywordColumnInfo);
        if (skillNameTable == null || skillDescriptionTable == null || skillKeywordTable == null)
        {
            Debug.Log("skill Script table을 읽어오지 못했습니다.");
            return;
        }
        _language = UIManager.instance.scriptLanguage;
        _skillNameScripts = new List<SkillNameScript>();
        for (int i = 0; i < skillNameTable.Count; i++)
        {
            SkillNameScript script = new SkillNameScript(i, skillNameTable[i][(int)_language]);
            _skillNameScripts.Add(script);
        }

        _skillDescriptionScripts = new List<SkillDescriptionScript>();
        for (int i = 0; i < skillDescriptionTable.Count; i++)
        {
            SkillDescriptionScript script = new SkillDescriptionScript(i, skillDescriptionTable[i][(int)_language]);
            _skillDescriptionScripts.Add(script);
        }
        _skillKeywordScripts = new List<KeywordScript>();
        for (int i = 0; i < skillKeywordTable.Count; i++)
        {
            KeywordScript script = new KeywordScript(int.Parse(skillKeywordTable[i][0]), skillKeywordTable[i][(int)_language], skillKeywordTable[i][(int)ScriptLanguage.English], skillKeywordTable[i][(int)_language + 3], skillKeywordTable[i][3] == "1");
            _skillKeywordScripts.Add(script);
        }
    }

    /// <summary>
    /// 존재하는 모든 스킬들을 반환합니다.
    /// </summary>
    /// <returns> 존재하는 모든 스킬들을 담은 Skill리스트 </returns>
    public List<Skill> GetAllSkills() 
    {
        return _skills;
    }
    /// <summary>
    /// 플레이어가 습득한 스킬들만 반환합니다.
    /// </summary>
    /// <returns> 플레이어가 습득한 스킬들을 담은 Skill리스트 </returns>
    public List<Skill> GetAllLearnedSkills()
    {
        List<Skill> learnedSkills = new List<Skill>();
        for (int i = 0; i < _skills.Count; i++)
        {
            if (!_skills[i].isLearned) continue;
            learnedSkills.Add(_skills[i]);
        }
        return learnedSkills;
    }
    /// <summary>
    /// 해당 고유번호 또는 이름를 가진 스킬을 반환합니다.
    /// </summary>
    /// <param name="index"> 반환하고자 하는 스킬의 고유번호 또는 이름 </param>
    /// <returns> 
    /// 해당 고유번호나 이름을 가진 스킬이 존재한다면 해당 스킬을 반환합니다.
    /// 아니라면 null을 반환합니다.
    /// </returns>
    public Skill GetSkill(int index)
    {
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].skillInfo.index == index)
            {
                return _skills[i];
            }
        }
        Debug.Log("해당 인덱스의 스킬을 찾지 못했습니다. 인덱스: " + index);
        return null;
    }
    /// <summary>
    /// 해당 고유번호 또는 이름를 가진 스킬을 반환합니다.
    /// </summary>
    /// <param name="index"> 반환하고자 하는 스킬의 고유번호 또는 이름 </param>
    /// <returns> 
    /// 해당 고유번호나 이름을 가진 스킬이 존재한다면 해당 스킬을 반환합니다.
    /// 아니라면 null을 반환합니다.
    /// </returns>
    public Skill GetSkill(string name)
    {
        int idx = -1;
        for (int i = 0; i < _skillNameScripts.Count; i++) 
        {
            if (_skillNameScripts[i].name == name) idx = i;
        }
        if (idx == -1) 
        {
            Debug.Log("해당 이름의 스킬을 찾지 못했습니다. 이름: " + name);
            return null;
        }
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].skillInfo.nameIndex == idx)
            {
                return _skills[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 스킬을 습득합니다.
    /// </summary>
    /// <param name="index"> 습득하고자 하는 스킬의 고유번호 </param>
    /// <param name="isFreeLearn"></param>
    /// <returns>
    /// 스킬 습득조건이 충족되지 않았거나, 스킬 포인트가 부족한 경우 스킬은 습득되지 않고 false를 반환합니다.
    /// 정상적으로 스킬을 습득한 경우 true를 반환합니다.
    /// </returns>
    public bool LearnSkill(int index, bool isFreeLearn = false) 
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null) return false;

        List<Skill> learnedSkill = GetAllLearnedSkills();
        foreach (Skill skill in learnedSkill) 
        {
            if (skill.skillInfo.index == index) 
            {
                Debug.Log("동일 스킬 습득 오류");
                return false;
            }
        }
        
        for (int i = 0; i < _skills.Count; i++) 
        {
            if (_skills[i].skillInfo.index == index) 
            {
                if (!_skills[i].isLearnable) 
                { 
                    Debug.Log("습득 조건이 충족되지 않은 스킬입니다.");
                    return false; 
                }
                if (!IsEnoughSkillPoint() && !isFreeLearn) 
                {
                    Debug.Log("스킬 포인트가 부족합니다.");
                    return false;
                }

                _skillPoint -= isFreeLearn ? 0 : REQUIRED_SKILL_POINT;
                _skills[i].LearnSkill();
                PlayerEvents.OnLearnedSkill?.Invoke(_skills[i].skillInfo);
                GameManager.instance.AddPlayerSkillListElement(_skills[i].skillInfo);
                passiveDB.GetPassive(_skills[i].skillInfo.index, player);
                player.SetPassive(GameManager.instance.playerPassiveIndexList.Select(idx => passiveDB.GetPassive(idx, player)).ToList());
                break;
            }
        }
        for (int i = 0; i < _skills.Count; i++)
        {
            _skills[i].UpdateIsLearnable(_skills);
        }

        return true;
    }
    /// <summary>
    /// 현재 소지한 스킬 포인트를 반환합니다.
    /// </summary>
    /// <returns> 현재 스킬 포인트 </returns>
    public int GetSkillPoint() 
    {
        return _skillPoint;
    }
    public void AddSkillPoint(int sp) 
    {
        _skillPoint += sp;
        PlayerEvents.OnIncSkillPoint?.Invoke();
    }
    /// <summary>
    /// 현재 스킬 포인트가 스킬을 배울 만큼 충분한 지 반환합니다.
    /// </summary>
    /// <returns>
    /// 현재 스킬 포인트가 필요한 스킬 포인트보다 작다면 false를 반환합니다.
    /// 아니라면 true를 반환합니다.
    /// </returns>
    public bool IsEnoughSkillPoint()
    {
        return REQUIRED_SKILL_POINT <= _skillPoint;
    }
    public string GetSkillName(int skillIndex) 
    {
        Skill skill = GetSkill(skillIndex);
        if (skill == null || _skillNameScripts.Count <= skill.skillInfo.nameIndex) return "";
        return _skillNameScripts[skill.skillInfo.nameIndex].name;
    }
    public string GetSkillDescription(int skillIndex, out List<int> keywords)
    {
        Skill skill = GetSkill(skillIndex);
        if (skill == null || _skillDescriptionScripts.Count <= skill.skillInfo.tooltipIndex)
        {
            keywords = null;
            return null;
        }
        return _skillDescriptionScripts[skill.skillInfo.tooltipIndex].GetDescription(skillIndex, out keywords);
    }

    //delete and replace (search by name) later
    public KeywordScript GetSkillKeyword(int keywordIndex)
    {
        foreach (KeywordScript script in _skillKeywordScripts) 
        {
            if (script.index == keywordIndex) 
            {
                return script;
            }
        }
        return null;
    }
    public KeywordScript GetSkillKeyword(string keywordName)
    {
        foreach (KeywordScript script in _skillKeywordScripts)
        {
            if (script.Ename == keywordName)
            {
                return script;
            }
        }
        return null;
    }
    public KeywordScript FindSkillKeywordByName(string name)
    {
        foreach (KeywordScript script in _skillKeywordScripts)
        {
            if (script.Ename == name)
            {
                return script;
            }
        }
        Debug.Log("키워드 탐색 실패");
        return null;
    }
    public int FindUpgradeSkillIndex(SkillInfo sInfo, ActionType type)
    {
        if (sInfo.precedenceSkillInfo.Count == 0)
        {
            return -1;
        }
        else
        {
            foreach (SkillInfo pre in sInfo.precedenceSkillInfo)
            {
                int result = rFindSameActionTypeSkill(pre, type);
                if (result != -1)
                {
                    return result;
                }
            }
            return -1;
        }
    }
    private int rFindSameActionTypeSkill(SkillInfo sInfo, ActionType type)
    {
        if (sInfo.IsActive()) 
        {
            ActiveInfo aInfo = activeDB.GetActiveInfo(sInfo.index);
            if (aInfo.action == type) 
            {
                return sInfo.index;
            }
        }

        return FindUpgradeSkillIndex(sInfo, type);
    }
}
