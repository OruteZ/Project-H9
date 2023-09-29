using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatLevelUpUI : UISystem
{
    [SerializeField] private GameObject _statLevelUpWindow;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _statLevelUpTitleText;
    [SerializeField] private GameObject[] _statCardButtons;
    [SerializeField] private GameObject _statSelectButton;

    private bool isOpenUI = false;

    private int[] _statLevel = { 0, 0, 0 }; //Player에게 이관 후 저장 가능하게 해야 함.
    private string[] _statNames = { "Concentration", "Sight Range", "Speed" };
    public  int[] statIncreseValue = { 10, 1, 10 };
    public int selectedCardNumber { get; private set; }
    public bool isSelectedSomeCard { get; private set; }


    private int _appearSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        selectedCardNumber = -1;
        isSelectedSomeCard = false;
        ClosePlayerStatLevelUpUI();
    }

    // Update is called once per frame
    void Update()
    {
        float[] appearTargetValue = { 380, 0, 192 / 255.0f };
        float[] disappearTargetValue = 
        {
            Camera.main.pixelHeight / 2  + _statLevelUpTitleText.GetComponent<RectTransform>().sizeDelta.y / 2, 
            -(Camera.main.pixelHeight / 2 + _statCardButtons[0].GetComponent<RectTransform>().sizeDelta.y / 2),
            0
        };
        Vector3 pos;
        Color color;
        float[] targetValue;
        if (isOpenUI)
        {
            targetValue = appearTargetValue;
        }
        else
        {
            targetValue = disappearTargetValue;
        }
        pos = _statLevelUpTitleText.GetComponent<RectTransform>().localPosition;
        pos.y = CalculationLerpValue(pos.y, targetValue[0]);
        _statLevelUpTitleText.GetComponent<RectTransform>().localPosition = pos;
        if (pos.y == disappearTargetValue[0]) _statLevelUpTitleText.SetActive(false);

            for (int i = 0; i < _statCardButtons.Length; i++)
        {
            pos = _statCardButtons[i].GetComponent<RectTransform>().localPosition;
            pos.y = CalculationLerpValue(pos.y, targetValue[1]);
            _statCardButtons[i].GetComponent<RectTransform>().localPosition = pos;
            if (pos.y == disappearTargetValue[1]) _statCardButtons[i].SetActive(false);
        }

        color = _background.GetComponent<Image>().color;
        color.a = CalculationLerpValue(color.a, targetValue[2]);
        _background.GetComponent<Image>().color = color;
        if (color.a == disappearTargetValue[2]) _background.SetActive(false);

    }
    private float CalculationLerpValue(float a, float b)
    {
        float threshold = 0.01f;
        if (Mathf.Abs(a - b) < threshold)
        {
            a = b;
        }
        else
        {
            a = Mathf.Lerp(a, b, Time.deltaTime * _appearSpeed);
        }
        return a;
    }

    public void OpenPlayerStatLevelUpUI() 
    {
        if (isOpenUI) return;
        int cnt = 0;
        for (int i = 0; i < _statCardButtons.Length; i++)
        {
            if (_statCardButtons[i].GetComponent<PlayerStatLevelUpElement>().isLevelUpFully)
            {
                cnt++;
            }
        }
        Debug.Log(cnt);
        if (cnt >= _statCardButtons.Length) return;

        OpenUI();
        isOpenUI = true;
        selectedCardNumber = -1;
        isSelectedSomeCard = false;

        //Set Title Position
        _statLevelUpTitleText.SetActive(true);
        Vector3 pos = _statLevelUpTitleText.GetComponent<RectTransform>().localPosition;
        pos.y = Camera.main.pixelHeight / 2  + _statLevelUpTitleText.GetComponent<RectTransform>().sizeDelta.y / 2;
        _statLevelUpTitleText.GetComponent<RectTransform>().localPosition = pos;

        //Set Card Position
        for (int i = 0; i < _statCardButtons.Length; i++) 
        {
            pos = _statCardButtons[i].GetComponent<RectTransform>().localPosition;
            pos.y = -(Camera.main.pixelHeight / 2 + _statCardButtons[i].GetComponent<RectTransform>().sizeDelta.y / 2 * (i + 1));
            _statCardButtons[i].GetComponent<RectTransform>().localPosition = pos;
            _statCardButtons[i].GetComponent<PlayerStatLevelUpElement>().SetPlayerStatLevelUpCard(_statLevel[i], _statNames[i]);
        }
        _statSelectButton.GetComponent<PlayerStatLevelUpSelectButton>().InitPlayerStatLevelUpSelectButton();

        //Set Background Color
        _background.SetActive(true);
        Color color = _background.GetComponent<Image>().color;
        color.a = 0;
        _background.GetComponent<Image>().color = color;

        _statSelectButton.SetActive(true);

        _statLevelUpWindow.SetActive(true);
    }
    private void ClosePlayerStatLevelUpUI() 
    {
        isOpenUI = false;
        for (int i = 0; i < _statCardButtons.Length; i++)
        {
            _statCardButtons[i].GetComponent<PlayerStatLevelUpElement>().ClearPlayerStatLevelUpUICard(_statLevel[i]);
        }
        _statSelectButton.SetActive(false);

        CloseUI();
    }

    public void ClickStatCard(int cardNumber, bool isSelected) 
    {
        if (isSelectedSomeCard && selectedCardNumber != cardNumber) return;
        isSelectedSomeCard = isSelected;
        if (isSelected)
        {
            selectedCardNumber = cardNumber;
        }
        else
        {
            selectedCardNumber = -1;
        }
    }
    public void ClickSelectButton() 
    {
        if (!isSelectedSomeCard) return;

        string levelUpStatName = _statCardButtons[selectedCardNumber].GetComponent<PlayerStatLevelUpElement>().statName;
        if (levelUpStatName == "Concentration") 
        {
            _statLevel[0]++;
            //FieldSystem.unitSystem.GetPlayer().   //스탯 변환 어케 함?
        }
        else if (levelUpStatName == "Sight Range")
        {
            _statLevel[1]++;
        }
        else if (levelUpStatName == "Speed")
        {
            _statLevel[2]++;
        }

        ClosePlayerStatLevelUpUI();
    }
}
