using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : UISystem
{
    [SerializeField] private GameObject _weaponNameText;
    [SerializeField] private GameObject _weaponImage;
    [SerializeField] private GameObject _weaponMagazine;

    [SerializeField] private Sprite[] _tmpWeaponImage;

    // Start is called before the first frame update
    void Start()
    {
        SetEquipmentUI();
        UIManager.instance.onWeaponChanged.AddListener(SetEquipmentUI);
    }

    public void SetEquipmentUI() 
    {
        WeaponData data = FieldSystem.unitSystem.GetWeaponData(GameManager.instance.playerWeaponIndex);

        _weaponNameText.GetComponent<TextMeshProUGUI>().text = data.weaponNameIndex.ToString();
        int imgIdx = 0;
        switch (data.type) 
        {
            case WeaponType.Revolver:
                {
                    imgIdx = 0;
                    break;
                }
            case WeaponType.Repeater:
                {
                    imgIdx = 1;
                    break;
                }
            case WeaponType.Shotgun:
                {
                    imgIdx = 2;
                    break;
                }
        }
        _weaponImage.GetComponent<Image>().sprite = _tmpWeaponImage[imgIdx];
        _weaponMagazine.GetComponent<PlayerMagazineUI>().SetMagazineUI(true);
    }
    private void ClearEquipmentUI()
    {
        _weaponNameText.GetComponent<TextMeshProUGUI>().text = "Null";
        _weaponImage.GetComponent<Image>().sprite = null;
        _weaponMagazine.GetComponent<PlayerMagazineUI>().SetMagazineUI(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
