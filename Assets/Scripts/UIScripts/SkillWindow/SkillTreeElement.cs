using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 트리 안에서 각각의 Skill요소를 구성하는 스킬트리UI의 기능을 구현한 클래스
/// </summary>
public class SkillTreeElement : UIElement
{
    //개선 필요
    //현재는 인스펙터 창에서 스킬 인덱스와 선행스킬연결UI를 하나하나 지정해주어야 하는데, 더 좋은 방법을 찾아서 변경해야 할 듯

    public int skillIndex;
    [SerializeField] private GameObject[] _precedenceLine;  //이름이 혼동된다. 해당 코드에서 가리키는 스킬을 습득했을 경우에 활성화해야하는 선행스킬연결UI임.
                                                            //아예 이걸 새 코드로 분리해야 할지도...

    [SerializeField] private Image _effectImage;
    [SerializeField] private Image _ButtonImage;
    [SerializeField] private Image _SkillImage;

    private SkillIcon _skillIcon;
    private void Start()
    {
        _skillIcon = _SkillImage.GetComponent<SkillIcon>();
        _skillIcon.SetSkillIndex(skillIndex);
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].transform.GetChild(0).GetComponent<Image>().color = new Color32(199, 94, 8, 255);
        }
    }

    /// <summary>
    /// 스킬트리UI 클릭 시 실행됩니다.
    /// skillUI에게 현재 스킬트리UI가 가리키는 스킬 정보에 관한 툴팁을 띄우라는 명령을 보냅니다.
    /// </summary>
    public void OnSkillUIBtnClick()
    {
        UIManager.instance.skillUI.ClickSkillUIButton(GetComponent<RectTransform>().position, skillIndex);
    }

    /// <summary>
    /// 스킬트리UI의 상태(습득 가능, 습득 불가, 습득 완료)를 지정합니다.
    /// skillUI에서 스킬트리UI들의 상태를 갱신할 때 실행됩니다.
    /// </summary>
    /// <param name="state"> 스킬트리UI의 상태 </param>
    public void SetSkillButtonEffect(int state) 
    {
        Color32[] effectColor =
        {
            UICustomColor.SkillIconNotLearnedColor,
            UICustomColor.SkillIconLearnableColor,
            UICustomColor.SkillIconLearnedColor
        };
        _effectImage.color = effectColor[state];
    }
    /// <summary>
    /// 스킬트리UI에 연결된 "해당 스킬트리UI가 가리키는 스킬을 배워야만 활성화되는 선행스킬연결UI"들의 상태를 설정합니다.
    /// </summary>
    public void SetSkillArrow()
    {
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].transform.GetChild(0).GetComponent<Image>().color = UICustomColor.SkillIconLearnedColor;
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
}
