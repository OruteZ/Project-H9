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
    void SetMagazineText() 
    {
        Weapon weapon = FieldSystem.unitSystem.GetPlayer().weapon;
        _magazineText.GetComponent<TextMeshProUGUI>().text = weapon.currentEmmo.ToString() + " / " + weapon.maxEmmo.ToString();
    }
}
