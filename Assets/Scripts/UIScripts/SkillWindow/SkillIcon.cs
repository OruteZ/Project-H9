using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 아이콘을 표시하는 UI 기능을 구현한 클래스
/// 스킬창 UI 및 캐릭터창 UI 모두에 사용하여,
/// 스킬창 UI에서는 SkillTreeElement의 하위 클래스로서 사용하고
/// 캐릭터창 UI에서는 단순히 이미지 표시 기능만을 수행한다.
/// 아직 미구현.
/// </summary>
public class SkillIcon : MonoBehaviour
{
    private int _skillIndex;

    /// <summary>
    /// 스킬 아이콘이 어떤 스킬을 표시할 지 지정합니다.
    /// </summary>
    /// <param name="index"> 스킬 고유번호 </param>
    public void GetSkillIndex(int index) 
    {
        _skillIndex = index;
        //FindIconImage();
    }

    private void FindIconImage()
    {
        SkillManager skillManager = SkillManager.instance;
        Sprite sprite = Resources.Load("Images/" + skillManager.GetSkill(_skillIndex).skillInfo.iconNumber) as Sprite;
        this.GetComponent<Image>().sprite = sprite;
    }
    /// <summary>
    /// 스킬 아이콘을 마우스오버할 때 실행됩니다.
    /// 행동선택버튼 구현할 때 썼던 기능 활용해서 다시 작성할 예정.
    /// </summary>
    public void OnSkillUIButtonOver()
    {
        //_uiManager._skillUI.ClickSkillUIButton(this.gameObject.transform, skillIndex);
    }
}
