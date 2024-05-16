using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dynamite : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _duration;

    public void SetDestination(Vector3Int hexPosition, Action Callback)
    {
        Vector3 worldPosition = Hex.Hex2World(hexPosition);

        StartCoroutine(MoveTo(worldPosition, Callback));
    }

    private IEnumerator MoveTo(Vector3 targetPosition, Action Callback)
    {
        //포물선을 그리며 targetPosition으로 이동
        float duration = 1f;

        Vector3 startPosition = transform.position;
        Vector3 controlPosition = startPosition + (targetPosition - startPosition) / 2 + Vector3.up * 5;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / duration;

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y = Mathf.Sin(progress * Mathf.PI) * 5;

            transform.position = currentPosition;

            yield return null;
        }

        Explode();
        Callback();
    }

    public void Explode()
    {
        //null check 
        if (_explosionPrefab == null)
        {
            Debug.LogError("Explosion Prefab is null");
            return;
        }

        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
