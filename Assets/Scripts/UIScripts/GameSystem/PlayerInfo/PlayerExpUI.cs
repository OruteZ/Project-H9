using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerExpUI : UIElement
{
    [SerializeField] private GameObject _playerExpBar;
    [SerializeField] private GameObject _playerLevelUpBar;

    private float _currentExpText, _targetCurrentExpText, _maxExpText;
    private bool isLastChange = false;

    // Start is called before the first frame update
    void Start()
    {
        _maxExpText = GameManager.instance.GetMaxExp();
        _currentExpText = 0;
        _targetCurrentExpText = 0;
        _playerExpBar.GetComponent<PlayerExpBar>().SetPlayerExpBar(0, "0 / 100");
        UIManager.instance.onPlayerStatChanged.AddListener(() => SetPlayerExpUI(GameManager.instance.curExp));
        //_playerLevelUpBar.SetActive(false);
    }
    void Update()
    {
        if (LerpCalculation.CalculateLerpValue(ref _currentExpText, _targetCurrentExpText, 5, 0.5f))
        {
            isLastChange = false;
            UpdateExpBar();
        }
        else 
        {
            if (!isLastChange) 
            {
                isLastChange = true;
                UpdateExpBar();
            }
        }
        if (_currentExpText >= _maxExpText)
        {
            _targetCurrentExpText -= _maxExpText;
            _maxExpText += 100;
            _currentExpText = 0;
            StopAllCoroutines();
            StartCoroutine(ShowLevelUpText());
            UpdateExpBar();
        }
    }
    private void UpdateExpBar()
    {
        float expSliderValue = _currentExpText / _maxExpText;
        string expText = ((int)_currentExpText).ToString() + " / " + ((int)_maxExpText).ToString();

        _playerExpBar.GetComponent<PlayerExpBar>().SetPlayerExpBar(expSliderValue, expText);
    }
    public void SetPlayerExpUI(int targetCurExp)
    {
        _maxExpText = GameManager.instance.GetMaxExp();
        _targetCurrentExpText = targetCurExp;
    }
    IEnumerator ShowLevelUpText() 
    {
        //_playerLevelUpBar.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        //_playerLevelUpBar.SetActive(false);
        yield break;
    }
}
