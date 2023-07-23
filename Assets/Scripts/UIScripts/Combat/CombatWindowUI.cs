using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatWindowUI : UISystem
{
    public CombatActionUI combatActionUI { get; private set; }
    public MagazineUI magazineUI { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        combatActionUI = GetComponent<CombatActionUI>();
        magazineUI = GetComponent<MagazineUI>();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        combatActionUI.OpenUI();
        magazineUI.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        combatActionUI.CloseUI();
        magazineUI.CloseUI();
    }
}
