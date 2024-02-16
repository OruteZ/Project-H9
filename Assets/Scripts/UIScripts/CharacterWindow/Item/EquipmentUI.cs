using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : UISystem
{
    [SerializeField] private GameObject _equippedWeaponCell;

    [SerializeField] private GameObject _weaponNameText;
    [SerializeField] private GameObject _weaponImage;
    [SerializeField] private GameObject _weaponMagazine;

    [SerializeField] private Sprite[] _tmpWeaponImage;

    // Start is called before the first frame update
    void Start()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();

        //fix later - find weapon's Iitem
        IItem playerWeapon = null;//player.weapon;
        _equippedWeaponCell.GetComponent<InventoryUIElement>().SetInventoryUIElement(playerWeapon);
        SetEquipmentUI();
    }

    public void SetEquipmentUI() 
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        Weapon weapon = player.weapon;
        Debug.Log(weapon.nameIndex);

        _weaponNameText.GetComponent<TextMeshProUGUI>().text = weapon.nameIndex.ToString(); //fix later - load name by index
        int imgIdx = 0;
        switch (weapon.GetWeaponType()) 
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
