using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectButton : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    public void DestroyTarget()
    {
        if (Target != null)
            Destroy(Target);
    }
}
