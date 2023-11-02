using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ų Ʈ�� �ȿ��� ������ Skill��Ҹ� �����ϴ� ��ųƮ��UI�� ����� ������ Ŭ����
/// </summary>
public class SkillTreeElement : UIElement
{
    [SerializeField] private int skillIndex;
    [SerializeField] private List<GameObject> _postLine;

    [SerializeField] private Image _effectImage;
    [SerializeField] private Image _SkillImage;

    private SkillIcon _skillIcon;

    public float effectSpeed { get; private set; }
    private void Start()
    {
        ClearPostArrowList();
        effectSpeed = 10;
        StartCoroutine(EffectColorChange((int)SkillUI.LearnStatus.NotLearnable, 0));

        _skillIcon = _SkillImage.GetComponent<SkillIcon>();
        _skillIcon.SetSkillIndex(skillIndex);
    }

    /// <summary>
    /// ��ųƮ��UI Ŭ�� �� ����˴ϴ�.
    /// skillUI���� ���� ��ųƮ��UI�� ����Ű�� ��ų ������ ���� ������ ����� ������ �����ϴ�.
    /// </summary>
    public void OnSkillUIBtnClick()
    {
        UIManager.instance.skillUI.ClickSkillUIButton(GetComponent<RectTransform>().position, skillIndex);
    }

    /// <summary>
    /// ��ųƮ��UI�� ����(���� ����, ���� �Ұ�, ���� �Ϸ�)�� �����մϴ�.
    /// skillUI���� ��ųƮ��UI���� ���¸� ������ �� ����˴ϴ�.
    /// </summary>
    /// <param name="state"> ��ųƮ��UI�� ���� </param>
    public void SetSkillButtonEffect(int state) 
    {
        StartCoroutine(EffectColorChange(state, 1 / effectSpeed));
    }
    IEnumerator EffectColorChange(int state, float speed)
    {
        Color32[] effectColor =
        {
            UICustomColor.SkillIconNotLearnedColor,
            UICustomColor.SkillIconLearnableColor,
            UICustomColor.SkillIconLearnedColor
        };
        yield return new WaitForSeconds(speed);
        _effectImage.color = effectColor[state];
        yield break;
    }
    /// <summary>
    /// ��ųƮ��UI�� ����� "�ش� ��ųƮ��UI�� ����Ű�� ��ų�� ����߸� Ȱ��ȭ�Ǵ� ���ེų����UI"���� ���¸� �����մϴ�.
    /// </summary>
    public void SetSkillArrow()
    {
        for (int i = 0; i < _postLine.Count; i++)
        {
            _postLine[i].GetComponent<SkillTreeArrow>().ProgressArrow();
        }
    }
    /// <summary>
    /// ��ųƮ���� ����Ű�� �ִ� ��ų�� ������ȣ�� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns> ��ųƮ���� ����Ű�� �ִ� ��ų�� ������ȣ </returns>
    public int GetSkillUIIndex() 
    {
        return skillIndex;
    }
    public GameObject FindSkillNode(int index) 
    {
        if (index == skillIndex)
        {
            return this.gameObject;
        }
        else 
        {
            foreach (GameObject arrow in _postLine) 
            {
                GameObject postNode = arrow.GetComponent<SkillTreeArrow>().GetPostSkillNode();
                GameObject findResult = postNode.GetComponent<SkillTreeElement>().FindSkillNode(index);
                if (findResult != null) 
                {
                    return findResult;
                }
            }
            return null;
        }
    }
    public List<GameObject> GetPostArrow() 
    {
        return _postLine;
    }
    public void AddPostArrow(GameObject arrow) 
    {
        _postLine.Add(arrow);
    }
    private void ClearPostArrowList()
    {
        for (int i = _postLine.Count - 1; i >= 0; i--)
        {
            if (_postLine[i] is null)
            {
                _postLine.RemoveAt(i);
            }
        }
    }
}