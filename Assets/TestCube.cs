using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    private void Start()
    {
        MeshCut.Cut(gameObject, transform.position, Vector3.left, GetComponent<MeshRenderer>().material);
    }
}
