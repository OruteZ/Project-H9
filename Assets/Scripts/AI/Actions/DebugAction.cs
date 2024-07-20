using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAction : H9Action
{
    public string message;
    
    public override IEnumerator Execute()
    {
        Debug.Log(message);
        yield break;
    }
}