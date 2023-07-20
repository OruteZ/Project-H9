using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagazineUI : UISystem
{
    [SerializeField] private GameObject _magazineText;
    private Weapon _weapon;

    void SetMagazineText() 
    {
        Weapon weapon = FieldSystem.unitSystem.GetPlayer().weapon;
        //_magazineText.GetComponent<TextMeshProUGUI>().text = weapon.currentAmmo.ToString() + " / " + weapon.maxAmmo.ToString();
    }
}
