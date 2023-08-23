using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 전투 중 현재 시작되는 턴의 주인을 화면 중앙에 텍스트로 표시하는 기능을 수행하는 클래스
/// </summary>
public class StartTurnTextUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    // Start is called before the first frame update
    void Start()
    {
        InitTurnText();
    }

    private void InitTurnText()
    {
        turnText.text = "";
        turnText.enabled = false;
    }

    /// <summary>
    /// 현재 시작되는 턴의 주인을 Unit 클래스로 입력받으면 그에 맞는 텍스트를 화면에 출력합니다.
    /// 전투 씬의 턴이 시작될 때마다 실행됩니다.
    /// </summary>
    /// <param name="unit"> 현재 시작되는 턴 주인 </param>
    public void SetStartTurnTextUI(Unit unit) 
    {
        if (!GameManager.instance.CompareState(GameState.Combat)) return;

        StopAllCoroutines();
        if (unit is Player)
        {
            StartCoroutine(ShowTurnText("Your Turn!", 2.0f));
            
        }
        else
        {
            StartCoroutine(ShowTurnText("Enemy Turn", 2.0f));
        }
    }

    IEnumerator ShowTurnText(string text, float time) 
    {
        while (true)
        {
            turnText.enabled = true;
            turnText.text = text;
            yield return new WaitForSeconds(time);
            InitTurnText();
            yield break;
        }
    }
}
