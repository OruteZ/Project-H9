using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagazineUI : UISystem
{
    [SerializeField] private GameObject _magazineText;

    private void Update()
    {
        //for test
        SetMagazineText();
    }
    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    public void SetMagazineText() 
    {
        Weapon weapon = FieldSystem.unitSystem.GetPlayer().weapon;
        _magazineText.GetComponent<TextMeshProUGUI>().text = weapon.currentEmmo.ToString() + " / " + weapon.maxEmmo.ToString();
    }
}
