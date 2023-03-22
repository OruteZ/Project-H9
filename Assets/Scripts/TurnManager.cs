using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private IEnumerator ContinueTurn()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return i;
        }
    }

    private IEnumerator _it;
    // Start is called before the first frame update
    void Start()
    {
        _it = ContinueTurn();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_it.Current);
        _it.MoveNext();
    }
}
