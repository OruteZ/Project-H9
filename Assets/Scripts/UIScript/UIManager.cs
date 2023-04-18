using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;

public class UIManager : Singleton<UIManager>
{
    public CombatUI combatUI;
    
    public bool IsMouseOverUI()
    {
        return false; 
    }
}
