using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatUIInfo
{
    private Dictionary<string, string> _statNameTransleation = new Dictionary<string, string>()
    {
            {"HP",                      "체력" },
            {"Concentration",           "집중력" },
            {"Sight Range",             "시야 범위" },
            {"Speed",                   "속도" },
            {"Action Point",            "행동 포인트" },
            {"Additional Hit Rate",     "추가 명중률" },
            {"Critical Chance",         "치명타 확률" },
            {"Additional Damage",       "추가 데미지" },
            {"Additional Range",        "추가 사거리" },
            {"Critical Damage",         "치명타 데미지" },
            {"Name",                    "무기 이름" },
            {"Ammo",                    "무기 탄창 용량" },
            {"Damage",                  "무기 데미지" },
            {"Range",                   "무기 사거리" },
            {"",                        "" },
    };
    //이걸 이런 식으로 여기 이러는 게 맞나?

    public string statName { get; private set; }
    public Dictionary<string, float> statValues { get; private set; }

    public CharacterStatUIInfo(string name) 
    {
        statName = name;
        statValues = new Dictionary<string, float>() 
        {
            {"CharacterStat",   0.0f},
            {"WeaponStat",      0.0f},
            {"SkillStat",       0.0f}
        };
    }

    public void SetStatValue(string statType, float value) 
    {
        if (!statValues.ContainsKey(statType)) 
        {
            Debug.LogError("잘못된 키 밸류 입력");
            return;
        }
        statValues[statType] = value;
    }
    public string GetTranslateStatName() 
    {
        return _statNameTransleation[statName];
    }
    public float GetFinalStatValue() 
    {
        float result = 0;
        foreach (float x in statValues.Values) 
        {
            result += x;
        }
        return result;
    }
    public string GetFinalStatValueString() 
    {
        float finalStat = GetFinalStatValue();
        if (statName == "") return "";
        if (statName == "Additional Hit Rate") return finalStat.ToString() + "%";
        if (statName == "Critical Chance") return ((int)(finalStat * 100)).ToString() + '%';



        return finalStat.ToString();
    }
}

/// <summary>
/// 캐릭터 정보 창에서 캐릭터의 스텟 및 장비하고 있는 무기의 스텟 정보를 표시하는 기능을 구현한 클래스
/// </summary>
public class CharacterStatUI : UISystem
{
    //Character Stat
    [Header("Character Stat UI")]
    [SerializeField] private GameObject _characterStatText;
    [SerializeField] private GameObject _weaponStatTextName;
    [SerializeField] private GameObject _weaponStatTextContents;

    [SerializeField] private GameObject _characterStatTexts;
    [SerializeField] private GameObject _weaponStatTexts;

    public GameObject _characterStatTooltip;
    private readonly List<string> _stats = new List<string>()
    {   "HP",
        "Concentration",
        "Sight Range",
        "Speed",
        "Action Point",
        "",
        "Additional Hit Rate",
        "Critical Chance",
        "Additional Damage",
        "Additional Range",
        "Critical Damage",
        "Name",
        "Ammo",
        "Damage",
        "Range"
    };
    public Dictionary<string, CharacterStatUIInfo> characterStatInfo { get; private set; }
    private void Start()
    {
        characterStatInfo = new Dictionary<string, CharacterStatUIInfo>();
        foreach (string str in _stats)
        {
            characterStatInfo.Add(str, new CharacterStatUIInfo(str));
        }
    }

    public override void OpenUI()
    {
        base.OpenUI();

        SetStatText();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    private void SetStatText()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();

        SetStatInfo(player);
        SetCharacterStatText();
        SetWeaponStatText();
    }
    private void SetStatInfo(Player player) 
    {
        UnitStat stat = player.GetStat();
        Weapon weapon = player.weapon;
        //player
        //basic stat
        characterStatInfo["HP"].SetStatValue("CharacterStat", stat.maxHp);
        //characterStatInfo["HP"].SetStatValue("WeaponStat", weapon.bonusStat.maxHp); //예시
        characterStatInfo["Concentration"].SetStatValue("CharacterStat", stat.concentration);
        characterStatInfo["Sight Range"].SetStatValue("CharacterStat", stat.sightRange);
        characterStatInfo["Speed"].SetStatValue("CharacterStat", stat.speed);
        characterStatInfo["Action Point"].SetStatValue("CharacterStat", stat.actionPoint);

        //bonus stat
        characterStatInfo["Additional Hit Rate"].SetStatValue("CharacterStat", stat.additionalHitRate);
        characterStatInfo["Additional Hit Rate"].SetStatValue("WeaponStat", weapon.hitRate);
        characterStatInfo["Critical Chance"].SetStatValue("CharacterStat", stat.criticalChance);
        characterStatInfo["Critical Chance"].SetStatValue("WeaponStat", weapon.criticalChance);

        WeaponType weaponType = weapon.GetWeaponType();
        if (weaponType is WeaponType.Revolver)
        {
            characterStatInfo["Additional Damage"].SetStatValue("CharacterStat", stat.revolverAdditionalDamage);
            characterStatInfo["Additional Range"].SetStatValue("CharacterStat", stat.revolverAdditionalRange);
            characterStatInfo["Critical Damage"].SetStatValue("CharacterStat", stat.revolverCriticalDamage);
        }
        else if (weaponType is WeaponType.Repeater)
        {
            characterStatInfo["Additional Damage"].SetStatValue("CharacterStat", stat.repeaterAdditionalDamage);
            characterStatInfo["Additional Range"].SetStatValue("CharacterStat", stat.repeaterAdditionalRange);
            characterStatInfo["Critical Damage"].SetStatValue("CharacterStat", stat.repeaterCriticalDamage);
        }
        else if (weaponType is WeaponType.Shotgun)
        {
            characterStatInfo["Additional Damage"].SetStatValue("CharacterStat", stat.shotgunAdditionalDamage);
            characterStatInfo["Additional Range"].SetStatValue("CharacterStat", stat.shotgunAdditionalRange);
            characterStatInfo["Critical Damage"].SetStatValue("CharacterStat", stat.shotgunCriticalDamage);
        }
        characterStatInfo["Critical Damage"].SetStatValue("WeaponStat", weapon.criticalDamage);

        //weapon
        characterStatInfo["Name"].SetStatValue("WeaponStat", weapon.nameIndex);
        characterStatInfo["Ammo"].SetStatValue("WeaponStat", weapon.maxAmmo);
        characterStatInfo["Damage"].SetStatValue("WeaponStat", weapon.weaponDamage);
        characterStatInfo["Range"].SetStatValue("WeaponStat", weapon.weaponRange);
    }
    private void SetCharacterStatText()
    {
        for (int i = 0; i < _characterStatTexts.transform.childCount; i++) 
        {
            _characterStatTexts.transform.GetChild(i)
                .GetComponent<CharacterStatTextElement>().SetCharacterStatText(characterStatInfo[_stats[i]]);
        }
    }
    private void SetWeaponStatText()
    {
        for (int i = 0; i < _weaponStatTexts.transform.childCount; i++)
        {
            _weaponStatTexts.transform.GetChild(i)
                .GetComponent<CharacterStatTextElement>().SetCharacterStatText(characterStatInfo[_stats[i + _characterStatTexts.transform.childCount]]);
        }
    }

    public void OpenCharacterTooltip(string name, float yPosition) 
    {
        _characterStatTooltip.GetComponent<CharacterTooltip>().SetCharacterTooltip(characterStatInfo[name], yPosition);
    }
    public void CloseCharacterTooltip(string name)
    {
        bool isTooltipMatched = (name == _characterStatTooltip.GetComponent<CharacterTooltip>().nameText);
        bool isTooltipMouseOver = (_characterStatTooltip.GetComponent<CharacterTooltip>().isMouseOver);
        if (!isTooltipMatched || isTooltipMouseOver) return;
        _characterStatTooltip.GetComponent<CharacterTooltip>().CloseUI();
    }
}


//HP:
//Concentration:
//Sight Range:
//Speed:
//Action Point:
//Additional Hit Rate:
//Critical Chance:

//Additional Damage:
//Additional Range:
//Critical Damage:

//Name:
//Ammo:
//Damage:
//Range:
//Additional Hit Rate:
//Critical Chance:
//Critical Damage: