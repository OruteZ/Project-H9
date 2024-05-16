using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField] private float _time;

    private void Start() {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine() {
        yield return new WaitForSeconds(_time);
        Destroy(gameObject);
    }
}
