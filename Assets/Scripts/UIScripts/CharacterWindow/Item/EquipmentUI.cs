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

    // Start is called before the first frame update
    void Start()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();

        //fix later - find weapon's Iitem
        IItem playerWeapon = null;//player.weapon;
        _equippedWeaponCell.GetComponent<InventoryUIElement>().SetInventoryUIElement(playerWeapon);
    }

    public void SetEquipmentUI() 
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        Weapon weapon = player.weapon;

        _weaponNameText.GetComponent<TextMeshProUGUI>().text = weapon.nameIndex.ToString(); //fix later - load name by index
        _weaponImage.GetComponent<Image>().sprite = null; // - how to find sprite?
        _weaponMagazine.GetComponent<PlayerMagazineUI>().SetMagazineUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
