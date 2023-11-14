using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitModel : MonoBehaviour
{
    #region Triggers
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");
    private static readonly int SHOOT = Animator.StringToHash("Shoot");
    private static readonly int RELOAD = Animator.StringToHash("Reload");
    private static readonly int MOVE = Animator.StringToHash("Move");
    private static readonly int GET_HIT1 = Animator.StringToHash("GetHit1");
    private static readonly int GET_HIT2 = Animator.StringToHash("GetHit2");
    private static readonly int DIE = Animator.StringToHash("Die");
    private static readonly int FANNING = Animator.StringToHash("Fanning");
    private static readonly int DYNAMITE = Animator.StringToHash("Dynamite");
    #endregion
    
    public Transform hand;
    public Transform back;
    public Transform waist;
    public Transform triggerFinger;
    public Transform head;
    
    public Animator animator;
    public Unit unit;

    public void Setup(Unit unit)
    {
        if (TryGetComponent(out animator) is false)
        {
            Debug.LogError("Animator is null");
            EditorApplication.isPaused = true;
        }

        this.unit = unit;
        _deadFlag = false;
        
        #region EVENTS

        unit.onHit.AddListener(OnHit);
        unit.onTurnStart.AddListener(OnStartTurn);

        #endregion
    }

    public void SetAnimator(WeaponType type)
    {
        animator.runtimeAnimatorController =
            (RuntimeAnimatorController)Resources.Load("Animator/" 
                                                      + (type is WeaponType.Null ? "Standing" : type) +
                                                      " Animator Controller");
        
        animator.SetTrigger(IDLE);
    }
    
    
    #region PRIVATE

    private bool _deadFlag = false;

    private void OnHit(Unit attacker, int damage)
    {
        if (_deadFlag) return;
        
        animator.SetTrigger(GET_HIT1);
        //if hp is 0, die
        if (unit.hp <= 0)
        {
            animator.SetBool(DIE, true);
            _deadFlag = true;
        }
    }

    private void OnStartTurn(Unit owner)
    {
        animator.SetTrigger(START_TURN);
    }
    
    #endregion
}
