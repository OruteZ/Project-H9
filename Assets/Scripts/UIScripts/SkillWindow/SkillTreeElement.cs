using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 트리 안에서 각각의 Skill요소를 구성하는 스킬트리UI의 기능을 구현한 클래스
/// </summary>
public class SkillTreeElement : Generic.Singleton<SkillTreeElement>
{
    //개선 필요
    //현재는 인스펙터 창에서 스킬 인덱스와 선행스킬연결UI를 하나하나 지정해주어야 하는데, 더 좋은 방법을 찾아서 변경해야 할 듯

    public int skillIndex;
    [SerializeField] private GameObject[] _precedenceLine;  //이름이 혼동된다. 해당 코드에서 가리키는 스킬을 습득했을 경우에 활성화해야하는 선행스킬연결UI임.
                                                            //아예 이걸 새 코드로 분리해야 할지도...

    //왜 여기서 정의하고 쓰면 색깔이 제대로 안나옴? 몰?루
    [SerializeField] private Color32[] _effectColor =
    {
        new Color32(0, 0, 0, 255),
        new Color32(255, 201, 18, 255),
        new Color32(72, 219, 18, 255)
    };
    [SerializeField] private Image _effectImage;
    [SerializeField] private Image _ButtonImage;
    [SerializeField] private Image _SkillImage;

    //1==습득불가, 2==습득가능, 3==습득완료 -> enum으로 변경해야 하나?
    //SkillIcon 코드로 기능 이전 예정?
    [SerializeField] private Sprite[] _effect = new Sprite[3];
    private void Start()
    {
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].transform.GetChild(0).GetComponent<Image>().color = new Color32(199, 94, 8, 255);
        }
    }

    private void Update()
    {
        if (_effectImage.color == Color.white) Debug.LogError("?????");
    }

    /// <summary>
    /// 스킬트리UI 클릭 시 실행됩니다.
    /// skillUI에게 현재 스킬트리UI가 가리키는 스킬 정보에 관한 툴팁을 띄우라는 명령을 보냅니다.
    /// </summary>
    public void OnSkillUIBtnClick()
    {
        UIManager.instance.skillUI.ClickSkillUIButton(this.gameObject.transform, skillIndex);
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
        new Color32(0, 0, 0, 255),
        new Color32(255, 201, 18, 255),
        new Color32(72, 219, 18, 255)
        };
        Debug.Log(effectColor[state]);
        _effectImage.color = effectColor[state];
    }
    /// <summary>
    /// 스킬트리UI에 연결된 "해당 스킬트리UI가 가리키는 스킬을 배워야만 활성화되는 선행스킬연결UI"들의 상태를 설정합니다.
    /// </summary>
    public void SetSkillArrow()
    {
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].transform.GetChild(0).GetComponent<Image>().color = _effectColor[1];
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
