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

    [SerializeField] private GameObject _equipmentParticle1;
    [SerializeField] private GameObject _equipmentParticle2;
    private const float PARTICLE_ROTATE_SPEED = 100;
    private bool _isParticleOn = false;

    // Start is called before the first frame update
    void Start()
    {
        ClearEquipmentParticle();
        SetEquipmentUI();
        PlayerEvents.OnWeaponChanged.AddListener((wpn) => SetEquipmentUI());
    }

    public void SetEquipmentUI() 
    {
        ItemData data = GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex);

        _weaponNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.itemDatabase.GetItemScript(data.nameIdx).GetName();
        //int imgIdx = 0;
        //switch (data.itemType) 
        //{
        //    case ItemType.Revolver:
        //        {
        //            imgIdx = 0;
        //            break;
        //        }
        //    case ItemType.Repeater:
        //        {
        //            imgIdx = 1;
        //            break;
        //        }
        //    case ItemType.Shotgun:
        //        {
        //            imgIdx = 2;
        //            break;
        //        }
        //}
        _weaponImage.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo(data.itemType.ToString());
        _weaponMagazine.GetComponent<PlayerMagazineUI>().SetMagazineUI(true);
    }
    private void ClearEquipmentUI()
    {
        _weaponNameText.GetComponent<TextMeshProUGUI>().text = "Null";
        _weaponImage.GetComponent<Image>().sprite = null;
        _weaponMagazine.GetComponent<PlayerMagazineUI>().SetMagazineUI(true);
    }

    private void Update()
    {
        if (_isParticleOn) 
        {
            _equipmentParticle1.GetComponent<RectTransform>().localEulerAngles += new Vector3(0, 0, Time.deltaTime * PARTICLE_ROTATE_SPEED);
        }
    }
    public void SetEquipmentParticle()
    {
        _isParticleOn = true;
        _equipmentParticle1.SetActive(true);
        _equipmentParticle2.SetActive(true);
    }
    public void ClearEquipmentParticle()
    {
        _isParticleOn = false;
        _equipmentParticle1.SetActive(false);
        _equipmentParticle2.SetActive(false);
    }
}
