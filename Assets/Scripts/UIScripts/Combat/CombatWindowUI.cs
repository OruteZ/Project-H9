using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CombatWindowUI : UISystem
{
    public CombatActionUI combatActionUI { get; private set; }
    public MagazineUI magazineUI { get; private set; }
    public EnemyHpUI enemyHpUI { get; private set; }
    public EnemyStatUI enemyStatUI { get; private set; }
    // Start is called before the first frame update
    private new void Awake()
    {
        base.Awake();
        
        combatActionUI = GetComponent<CombatActionUI>();
        magazineUI = GetComponent<MagazineUI>();
        enemyHpUI = GetComponent<EnemyHpUI>();
        enemyStatUI = GetComponent<EnemyStatUI>();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        combatActionUI.OpenUI();
        magazineUI.OpenUI();
        enemyHpUI.OpenUI();
        enemyStatUI.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        combatActionUI.CloseUI();
        magazineUI.CloseUI();
        enemyHpUI.CloseUI();
        enemyStatUI.CloseUI();
    }

    public void SetCombatUI()
    {
        combatActionUI.SetActionButtons();
        magazineUI.SetMagazineText();
        enemyHpUI.SetEnemyHpBars();
    }
}
