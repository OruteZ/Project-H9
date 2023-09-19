using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 정보 창에서 캐릭터가 습득한 스킬을 표시하는 기능을 구현한 클래스
/// </summary>
public class LearnedSkillUI : UISystem
{
    //Learned Skill UI
    [Header("Learned Skill UI")]
    [SerializeField] private GameObject _skillIconPrefab;
    [SerializeField] private GameObject _skillIconContainer;
    private List<GameObject> _skillIconUIs = new List<GameObject>();
    private readonly Vector3 ICON_INIT_POSITION = new Vector3(235, 280, 0);
    private const float ICON_INTERVAL = 100;

    private SkillManager _skillManager;

    // Start is called before the first frame update
    void Start()
    {
        _skillManager = SkillManager.instance;

        //Skill icon object pooling
        int skillCount = _skillManager.GetAllSkills().Count;
        for (int i = 0; i < skillCount; i++)
        {
            GameObject skillIcon = Instantiate(_skillIconPrefab, ICON_INIT_POSITION, Quaternion.identity, _skillIconContainer.transform);

            skillIcon.SetActive(false);
            _skillIconUIs.Add(skillIcon);
        }
    }
    public override void OpenUI()
    {
        base.OpenUI();
        SetLearnedSkiilInfoUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    /// <summary>
    /// 플레이어가 습득한 스킬들을 받아와서 캐릭터 창 UI에 요약해서 띄워주는 기능입니다.
    /// </summary>
    public void SetLearnedSkiilInfoUI()
    {
        for (int i = 0; i < _skillIconUIs.Count; i++)
        {
            _skillIconUIs[i].SetActive(false);
        }

        List<Skill> _skills = _skillManager.GetAllSkills();
        int cnt = 0;
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].isLearned)
            {
                Vector3 pos = ICON_INIT_POSITION;
                pos.x += cnt * ICON_INTERVAL;
                _skillIconUIs[i].transform.position = pos;
                _skillIconUIs[i].SetActive(true);

                cnt++;
            }
        }
        _skillIconContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(cnt * ICON_INTERVAL + 25, 100);
    }

}
