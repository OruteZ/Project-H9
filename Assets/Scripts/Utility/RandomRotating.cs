using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotating : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private Vector3 rotationAxis;

    private void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
