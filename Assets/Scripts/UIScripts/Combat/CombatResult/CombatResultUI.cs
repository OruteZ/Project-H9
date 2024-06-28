using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;

public class CombatResultUI : UISystem
{
    [SerializeField] private TMP_Text _ResultTitleText;
    [SerializeField] private TMP_Text _WinOrLoseText;
    [SerializeField] private TMP_Text _BootyTitleText;
    [SerializeField] private TMP_Text _BootyText;
    [SerializeField] private TMP_Text _CloseButtonText;

    [SerializeField] private Transform _ResultBootyItemParent;
    [SerializeField] private GameObject _ResultBootyItemPrefab;

    private string _winComment;
    private string _loseComment;
    private string _emptyBootyComment;

    private bool isPlayerWin;
    private int earnedExp;

    public void Start()
    {
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        //UIManager.instance.onTSceneChanged.AddListener(null);
        this.gameObject.SetActive(false);
    }

    public void OnClickResultWindowCloseButton()
    {
        CloseCombatResultUI();
        if (isPlayerWin)
        {
            BackToWorld();
        }
        else 
        {
            BackToMenu();
        }
    }
    private void BackToWorld()
    {
        GameManager.instance.FinishCombat();
    }

    private void BackToMenu()
    {
        // Find all GameObjects with DontDestroyOnLoad
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        
        SceneManager.LoadScene($"TitleScene");
    }

    private void OnCombatFinish(bool isPlayerWin)
    {
        this.isPlayerWin = isPlayerWin;

        _ResultTitleText.text = UIManager.instance.UILocalization[1];
        _winComment = UIManager.instance.UILocalization[2];
        _loseComment = UIManager.instance.UILocalization[3];
        _BootyTitleText.text = UIManager.instance.UILocalization[4];
        _CloseButtonText.text = UIManager.instance.UILocalization[5];
        _emptyBootyComment = UIManager.instance.UILocalization[7];

        Invoke(nameof(SetCombatResultUI), 2f);
    }

    private void SetCombatResultUI()
    {
        this.gameObject.SetActive(true);
        
        // Empty out "Result Booty Items"
        for (int i = _ResultBootyItemParent.childCount - 1; 0 <= i; i--)
        {
            Destroy(_ResultBootyItemParent.GetChild(i).gameObject);
        }

        if (isPlayerWin)
        {
            // set text
            _WinOrLoseText.text = _winComment;
            
            var exp = earnedExp;
            var gold = FieldSystem.unitSystem.rewardHelper.GetRewardGold();
            var bootyText = string.Empty;
            if (0 < exp && 0 < gold) bootyText = $"{exp} EXP, {gold} Gold";
            else if (0 < exp) bootyText = $"{exp} EXP";
            else if (0 < gold) bootyText = $"{gold} Gold";
            else bootyText = _emptyBootyComment;
            _BootyText.text = bootyText;

            // set item panel
            int[] itemidxs = FieldSystem.unitSystem.rewardHelper.GetRewardItemInfos();
            var itemSize = _ResultBootyItemPrefab.GetComponent<RectTransform>().sizeDelta;
            var intervalX = 5; // hard code.
            var positionY = 16; // hard code.
            var itemSizeSum = (itemidxs.Length * itemSize.x) + ((itemidxs.Length - 1) * intervalX) - ((itemSize.x / 2.0f) * 2);
            var startPositionX = -(itemSizeSum / 2.0f);
            for (int i = 0; i < itemidxs.Length; i++)
            {
                var index = itemidxs[i];
                var itemData = GameManager.instance.itemDatabase.GetItemData(index);
                // create and align
                var itemObj = Instantiate(_ResultBootyItemPrefab, _ResultBootyItemParent);
                itemObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPositionX + ((itemSize.x + intervalX) * i), positionY);
                // assign icon and name
                var itemIconObj = itemObj.transform.Find("ItemIcon");
                itemIconObj.GetComponent<Image>().sprite = itemData.icon;
                var itemNameObj = itemObj.transform.Find("ItemName");
                itemNameObj.GetComponent<TMP_Text>().text = GameManager.instance.itemDatabase.GetItemScript(index).GetName();
            }
        }
        else
        {
            _WinOrLoseText.text = _loseComment;
            _BootyText.text = _emptyBootyComment; // IF, player lose, get something, then fix it.
        }

        UIManager.instance.gameSystemUI.playerInfoUI.expUI.GetComponent<PlayerExpUI>().SetPlayerExpUI(GameManager.instance.LevelSystem.CurExp + earnedExp);
        earnedExp = 0;
    }
    private void CloseCombatResultUI()
    {
        this.gameObject.SetActive(false);
    }
    public void SetExpInformation(int eExp) 
    {
        earnedExp = eExp;
    }
}
