using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 트리 안에서 각각의 Skill요소를 구성하는 스킬트리UI의 기능을 구현한 클래스
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
    /// 스킬트리UI 클릭 시 실행됩니다.
    /// skillUI에게 현재 스킬트리UI가 가리키는 스킬 정보에 관한 툴팁을 띄우라는 명령을 보냅니다.
    /// </summary>
    public void OnSkillUIBtnClick()
    {
        UIManager.instance.skillUI.ClickSkillUIButton(skillIndex, GetComponent<RectTransform>().position);
    }

    /// <summary>
    /// 스킬트리UI의 상태(습득 가능, 습득 불가, 습득 완료)를 지정합니다.
    /// skillUI에서 스킬트리UI들의 상태를 갱신할 때 실행됩니다.
    /// </summary>
    /// <param name="state"> 스킬트리UI의 상태 </param>
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
    /// 스킬트리UI에 연결된 "해당 스킬트리UI가 가리키는 스킬을 배워야만 활성화되는 선행스킬연결UI"들의 상태를 설정합니다.
    /// </summary>
    public void SetSkillArrow()
    {
        for (int i = 0; i < _postLine.Count; i++)
        {
            _postLine[i].GetComponent<SkillTreeArrow>().ProgressArrow();
        }
    }
    /// <summary>
    /// 스킬트리가 가리키고 있는 스킬의 고유번호를 반환한다.
    /// </summary>
    /// <returns> 스킬트리가 가리키고 있는 스킬의 고유번호 </returns>
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
