using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private GameObject _characterStatusText;
    [SerializeField] private GameObject _weaponStatusText;
    public Image _characterImage;
    public Image _weaponImage;

    [SerializeField] private SkillManager _skillManager;
    public GameObject skillIconPrefab;
    private List<GameObject> skillIconUIs = new List<GameObject>();
    [SerializeField] private GameObject _iconScrollContents;
    private Vector3 ICON_INIT_POSITION = new Vector3(235, 280, 0);
    private float ICON_INTERVAL = 100;

    public enum ItemUIStatus
    {
        Weapon,
        Usable,
        Other
    };
    private ItemUIStatus _currentItemUIStatus = ItemUIStatus.Weapon;
    [SerializeField] private GameObject _weaponItemPanel;
    [SerializeField] private GameObject _usableItemPanel;
    [SerializeField] private GameObject _otherItemPanel;

    // Start is called before the first frame update
    void Start()
    {
        List<Skill> _skills = _skillManager.GetAllSkills();
        for (int i = 0; i < _skills.Count; i++) 
        {
            Vector3 pos = ICON_INIT_POSITION;
            pos.x += i * ICON_INTERVAL;
            GameObject skillIcon = Instantiate(skillIconPrefab, pos, Quaternion.identity);
            skillIcon.transform.SetParent(_iconScrollContents.transform);

            //skillIcon.GetComponent<SkillTreeElement>().skillIndex = _skill[i].skillInfo.index;
            //skillIcon.GetComponent<SkillTreeElement>()._uiManager = GetComponent<UiManager>();
            skillIcon.SetActive(false);
            skillIconUIs.Add(skillIcon);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void SetCharacterStatText()
    {

    }
    private void SetWeaponStatText()
    {

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

    public void ChangeItemUIStatus(ItemUIStatus status)
    {
        if (_currentItemUIStatus != status)
        {
            if (status == ItemUIStatus.Weapon)
            {
                ShowWeaponItems();
            }
            else if (status == ItemUIStatus.Usable)
            {
                ShowUsableItems();
            }
            else if (status == ItemUIStatus.Other)
            {
                ShowOtherItems();
            }
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
