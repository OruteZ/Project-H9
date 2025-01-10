using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class DeveloperKillAllCommand : MonoBehaviour
    {
        private void Awake()
        {
            #if !UNITY_EDITOR
            Destroy(this);
            #endif
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Debug.Log("DeveloperKillAllCommand: Kill all units");
                
                // Kill all units
                List<Unit> enemies = FieldSystem.unitSystem.units.FindAll(unit => unit is Enemy);
                foreach (Unit enemy in enemies)
                {
                    // create damage info
                    Damage dmg = new (
                        1000000,
                        1000000, 
                        Damage.Type.DEFAULT, 
                        null, 
                        null, 
                        enemy
                        );
                    
                    // apply damage
                    enemy.TakeDamage(dmg);
                }
            }
        }
    }
}