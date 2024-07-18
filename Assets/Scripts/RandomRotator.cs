using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    [ContextMenu("Randomize")]
    public void RotateRandomly()
    {
        // rotate y randomly
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
}
