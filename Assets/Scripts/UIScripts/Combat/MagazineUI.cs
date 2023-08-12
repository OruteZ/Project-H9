using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 플레이어의 현재 남은 탄창을 표시해주는 기능을 수행하는 클래스
/// </summary>
public class MagazineUI : UISystem
{
    [SerializeField] private GameObject _magazineText;

    private void Update()
    {
        //for test
        SetMagazineText();  //패닝액션 등과 같은 경우 기존 방식으로는 총알이 실시간으로 줄어들지 않기 때문에, 관련 문제가 해결될 때 까지 유지
    }
    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }

    /// <summary>
    /// 탄창 수 UI를 설정합니다.
    /// 액션을 시작될 때, 끝날 때, 선택할 때 실행됩니다.
    /// 현재 테스트용으로 매 프레임마다 업데이트되고 있습니다.
    /// </summary>
    public void SetMagazineText() 
    {
        Weapon weapon = FieldSystem.unitSystem.GetPlayer().weapon;
        _magazineText.GetComponent<TextMeshProUGUI>().text = weapon.currentAmmo.ToString() + " / " + weapon.maxAmmo.ToString();
    }
}
