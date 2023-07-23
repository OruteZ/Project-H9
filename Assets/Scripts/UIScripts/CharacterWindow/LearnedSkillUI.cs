using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnedSkillUI : UISystem
{
    //Learned Skill UI
    [Header("Learned Skill UI")]
    public GameObject skillIconPrefab;
    private List<GameObject> _skillIconUIs = new List<GameObject>();
    [SerializeField] private GameObject _iconScrollContents;
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
            GameObject skillIcon = Instantiate(skillIconPrefab, ICON_INIT_POSITION, Quaternion.identity, _iconScrollContents.transform);

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
        _iconScrollContents.GetComponent<RectTransform>().sizeDelta = new Vector2(cnt * ICON_INTERVAL + 25, 100);
    }

}
