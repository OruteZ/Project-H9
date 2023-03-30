using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _waiting = false;
    private IEnumerator _turnSystem;
    public Actor player;

    private void Awake()
    {
        _turnSystem = TurnPlay();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_waiting) return;
        _turnSystem.MoveNext();
    }
    
    private IEnumerator TurnPlay()
    {
        while (true)
        {
            _waiting = true;
            PlayerTurn();
            yield return null;
        }
    }

    private void PlayerTurn()
    {
        player.StartTurn();
    }

    public void ContinueTurn()
    {
        _waiting = false;
    }
}
