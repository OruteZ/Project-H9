using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 Hp아이콘UI 각각의 기능을 구현한 클래스
/// </summary>
public class PlayerHpUIElement : UIElement
{
    [SerializeField] private Sprite _playerHpFillSprite;
    [SerializeField] private Sprite _playerHpEmptySprite;

    /// <summary>
    /// Hp아이콘UI를 채웁니다.
    /// </summary>
    public void FillUI()
    {
        GetComponent<Image>().color = new Color(1, 0, 0, 1);
    }

    /// <summary>
    /// Hp아이콘UI를 비웁니다.
    /// </summary>
    public void EmptyUI()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
