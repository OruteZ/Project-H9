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

    [SerializeField] private GameObject _learneadEffect;
    [SerializeField] private Image _learnableEffectImage;
    [SerializeField] private Image _activeEffectImage;
    [SerializeField] private Image _SkillImage;

    private SkillIcon _skillIcon;

    public bool isRootNode = false;
    public float effectSpeed { get; private set; }
    private void Awake()
    {
    }
    private void Start()
    {
        ClearPostArrowList();
        effectSpeed = 10;
        _learneadEffect.GetComponent<Animator>().enabled = false;
        _learneadEffect.GetComponent<Image>().enabled = false;
        _learneadEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0.125f);
        _skillIcon = _SkillImage.GetComponent<SkillIcon>();
        _skillIcon.SetSkillIndex(skillIndex);
    }

    /// <summary>
    /// ��ųƮ��UI Ŭ�� �� ����˴ϴ�.
    /// skillUI���� ���� ��ųƮ��UI�� ����Ű�� ��ų ������ ���� ������ ����� ����� �����ϴ�.
    /// </summary>
    public void OnSkillUIBtnClick()
    {
        UIManager.instance.skillUI.ClickSkillUIButton(skillIndex, GetComponent<RectTransform>().position);
    }

    /// <summary>
    /// ��ųƮ��UI�� ����(���� ����, ���� �Ұ�, ���� �Ϸ�)�� �����մϴ�.
    /// skillUI���� ��ųƮ��UI���� ���¸� ������ �� ����˴ϴ�.
    /// </summary>
    /// <param name="state"> ��ųƮ��UI�� ���� </param>
    public void SetSkillButtonEffect(LearnStatus state)
    {
        StartCoroutine(ChangeEffectColor(state, 1 / effectSpeed));
    }

    IEnumerator ChangeEffectColor(LearnStatus state, float speed)
    {
        if (state != LearnStatus.NotLearnable)
        {
            _activeEffectImage.color = UICustomColor.Invisible;
        }
        else
        {
            _activeEffectImage.color = UICustomColor.Opaque;
        }

        Color32[] effectColor =
        {
            UICustomColor.SkillIconNotLearnedColor,
            UICustomColor.SkillIconLearnableColor,
            UICustomColor.SkillIconLearnedColor
        };
        if (state != LearnStatus.Learned)
        {
            yield return new WaitForSeconds(speed);
        }
        if (state != LearnStatus.NotLearnable)
        {
            _learneadEffect.GetComponent<Image>().enabled = true;
            _learneadEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0.125f);

            _learneadEffect.GetComponent<Animator>().enabled = (state == LearnStatus.Learnable);
        }
        else
        {
            _learneadEffect.GetComponent<Animator>().enabled = false;
            _learneadEffect.GetComponent<Image>().enabled = false;
            _learneadEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0.125f);
        }
        _learnableEffectImage.color = effectColor[(int)state];
        _SkillImage.color = effectColor[(int)state];
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
    public GameObject rFindSkillNodeAtChildren(int index) 
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
                GameObject findResult = postNode.GetComponent<SkillTreeElement>().rFindSkillNodeAtChildren(index);
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
    public void DeletePostArrow(GameObject arrow) 
    {
        _postLine.Remove(arrow);
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
