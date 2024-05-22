using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;

public class CombatResultUI : UISystem
{
    [SerializeField] private GameObject _ResultWindow;
    [SerializeField] private GameObject _WinOrLoseText;
    [SerializeField] private GameObject _ResultText;
    [SerializeField] private GameObject _CloseButton;

    private bool isPlayerWin;
    private int earnedExp;

    public void Start()
    {
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        UIManager.instance.onSceneChanged.AddListener(CloseCombatResultUI);
        //UIManager.instance.onTSceneChanged.AddListener(null);
        _ResultWindow.SetActive(false);
    }

    public void OnClickResultWindowCloseButton()
    {
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
        Invoke(nameof(SetCombatResultUI), 2f);
    }

    private void SetCombatResultUI()
    {
        _ResultWindow.SetActive(true);
        if (isPlayerWin)
        {
            _WinOrLoseText.GetComponent<TextMeshProUGUI>().text = "You Win!";
            string expStr = "Earned EXP: " + earnedExp + "\n";
            string goldStr = "Earned Gold: " + FieldSystem.unitSystem.rewardHelper.GetRewardGold() + "\n";
            string itemStr = "Earned Items: ";
            int[] itemidxs = FieldSystem.unitSystem.rewardHelper.GetRewardItemInfos();
            for (int i = 0; i < itemidxs.Length; i++)
            {
                itemStr += GameManager.instance.itemDatabase.GetItemScript(itemidxs[i]).GetName() + "\n\t";
            }
            _ResultText.GetComponent<TextMeshProUGUI>().text = expStr + goldStr + itemStr;
        }
        else
        {
            _WinOrLoseText.GetComponent<TextMeshProUGUI>().text = "Game Over";
        }

        UIManager.instance.gameSystemUI.playerInfoUI.expUI.GetComponent<PlayerExpUI>().SetPlayerExpUI(GameManager.instance.curExp + earnedExp);
        earnedExp = 0;
    }
    private void CloseCombatResultUI()
    {
        _ResultWindow.SetActive(false);
    }
    public void SetExpInformation(int eExp) 
    {
        earnedExp = eExp;
    }
}
