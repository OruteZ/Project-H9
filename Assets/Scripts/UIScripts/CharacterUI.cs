using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    //Character Stat
    [Header("Character Stat UI")]
    public GameManager gameManager; //unit system? game manager?
    [SerializeField] private GameObject _characterStatText;
    [SerializeField] private GameObject _weaponStatText;
    public Image _characterImage;
    public Image _weaponImage;

    //Learned Skill UI
    [Header("Learned Skill UI")]
    [SerializeField] private SkillManager _skillManager;
    public GameObject skillIconPrefab;
    private List<GameObject> skillIconUIs = new List<GameObject>();
    [SerializeField] private GameObject _iconScrollContents;
    private Vector3 ICON_INIT_POSITION = new Vector3(235, 280, 0);
    private float ICON_INTERVAL = 100;

    //Item UI
    [Header("Item UI")]
    [SerializeField] private GameObject _weaponItemPanel;
    [SerializeField] private GameObject _usableItemPanel;
    [SerializeField] private GameObject _otherItemPanel;

    private ItemManager.ItemCategory _currentItemUIStatus = ItemManager.ItemCategory.Weapon;

    // Start is called before the first frame update
    void Start()
    {
        //Skill icon object pooling
        List<Skill> _skills = _skillManager.GetAllSkills();
        for (int i = 0; i < _skills.Count; i++) 
        {
            Vector3 pos = ICON_INIT_POSITION;
            pos.x += i * ICON_INTERVAL;
            GameObject skillIcon = Instantiate(skillIconPrefab, pos, Quaternion.identity);
            skillIcon.transform.SetParent(_iconScrollContents.transform);

            skillIcon.SetActive(false);
            skillIconUIs.Add(skillIcon);
        }
    }

    public void OpenCharacterUI() 
    {
        SetStatText();
        SetLearnedSkiilInfoUI();
    }

    private void SetStatText()
    {
        UnitStat playerStat = gameManager.playerStat;
        Weapon weapon = gameManager.playerWeapon;
        WeaponType weaponType;
        //in test development
        if (weapon == null) { weaponType = WeaponType.Repeater; }
        else { weaponType = weapon.GetWeaponType(); }

        SetCharacterStatText(playerStat);
        SetWeaponStatText(playerStat, weaponType);
    }
    private void SetCharacterStatText(UnitStat playerStat)
    {
        string text = playerStat.hp.ToString() + '\n' +
                      playerStat.concentration.ToString() + '\n' +
                      playerStat.sightRange.ToString() + '\n' +
                      playerStat.speed.ToString() + '\n' +
                      playerStat.actionPoint.ToString() + '\n' +
                      playerStat.additionalHitRate.ToString() + '\n' +
                      playerStat.criticalChance.ToString();

        _characterStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(UnitStat playerStat, WeaponType weaponType)
    {
        string text = "";
        switch (weaponType) 
        {
            case WeaponType.Repeater: 
                {
                    text += playerStat.repeaterAdditionalDamage.ToString() + '\n' +
                            playerStat.repeaterAdditionalRange + '\n' +
                            playerStat.repeaterCriticalDamage;
                    break;
                }
            case WeaponType.Revolver:
                {
                    text += playerStat.revolverAdditionalDamage + '\n' +
                            playerStat.revolverAdditionalRange + '\n' +
                            playerStat.revolverCriticalDamage;
                    break;
                }
            case WeaponType.Shotgun:
                {
                    text += playerStat.shotgunAdditionalDamage + '\n' +
                            playerStat.shotgunAdditionalRange + '\n' +
                            playerStat.shotgunCriticalDamage;
                    break; 
                }
        }

        _weaponStatText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SetLearnedSkiilInfoUI()
    {
        for (int i = 0; i < skillIconUIs.Count; i++)
        {
            skillIconUIs[i].SetActive(false);
        }

        List<Skill> _skills = _skillManager.GetAllSkills();
        int cnt = 0;
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].isLearned)
            {
                Vector3 pos = ICON_INIT_POSITION;
                pos.x += cnt * ICON_INTERVAL;
                skillIconUIs[i].transform.position = pos;
                skillIconUIs[i].SetActive(true);

                cnt++;
            }
        }
        _iconScrollContents.GetComponent<RectTransform>().sizeDelta = new Vector2(cnt * 100 + 25, 100);
    }

    public void ChangeItemUIStatus(ItemManager.ItemCategory status)
    {
        if (_currentItemUIStatus != status)
        {
            if (status == ItemManager.ItemCategory.Weapon)
            {
                ShowWeaponItems();
            }
            else if (status == ItemManager.ItemCategory.Usable)
            {
                ShowUsableItems();
            }
            else if (status == ItemManager.ItemCategory.Other)
            {
                ShowOtherItems();
            }
            _currentItemUIStatus = status;
        }
    }
    private void ShowWeaponItems()
    {
        _weaponItemPanel.GetComponent<Image>().enabled = true;
        _usableItemPanel.GetComponent<Image>().enabled = false;
        _otherItemPanel.GetComponent<Image>().enabled = false;
    }
    private void ShowUsableItems()
    {
        _weaponItemPanel.GetComponent<Image>().enabled = false;
        _usableItemPanel.GetComponent<Image>().enabled = true;
        _otherItemPanel.GetComponent<Image>().enabled = false;
    }
    private void ShowOtherItems()
    {
        _weaponItemPanel.GetComponent<Image>().enabled = false;
        _usableItemPanel.GetComponent<Image>().enabled = false;
        _otherItemPanel.GetComponent<Image>().enabled = true;
    }
}
