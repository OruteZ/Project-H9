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

    private IEnumerator it;
    // Start is called before the first frame update
    void Start()
    {
        it = ContinueTurn();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(it.Current);
        it.MoveNext();
    }
}
