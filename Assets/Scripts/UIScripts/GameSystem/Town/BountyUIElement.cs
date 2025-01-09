using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BountyUIElement : UIElement
{
    [SerializeField] private GameObject _bountyPortrait;
    [SerializeField] private GameObject _bountyMoneyRewardText;
    [SerializeField] private GameObject _bountyExpRewardText;

    [SerializeField] private GameObject _bountyStartButton;

    private QuestInfo questInfo;
    public void SetBountyUIElement(QuestInfo qInfo)
    {
        questInfo = qInfo;
        LinkData lData = FieldSystem.unitSystem.GetLinkData(qInfo.GoalArg[0]);
        EnemyData eData = FieldSystem.unitSystem.GetEnemyData(lData.combatEnemy[0]);
        Texture2D enemyTexture = Resources.Load("UnitCapture/" + eData.modelName) as Texture2D;
        Sprite enemySpr = Sprite.Create(enemyTexture, new Rect(0, 0, enemyTexture.width, enemyTexture.height), new Vector2(0.5f, 0.5f));

        _bountyPortrait.GetComponent<Image>().sprite = enemySpr;
        _bountyMoneyRewardText.GetComponent<TextMeshProUGUI>().text = qInfo.MoneyReward.ToString() + "$ CASH";
        _bountyExpRewardText.GetComponent<TextMeshProUGUI>().text = qInfo.ExpReward.ToString() + "Wxp";

        _bountyStartButton.GetComponent<Button>().interactable = !qInfo.IsInProgress;
        if (qInfo.IsInProgress)
        {
            _bountyStartButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[1104];
        }
        else
        {
            _bountyStartButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[1103];
        }
    }

    public void OnClickBountyBtn() 
    {
        UIManager.instance.gameSystemUI.townUI.StartBounty(questInfo);
    }
}
