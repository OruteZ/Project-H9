using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DynamiteVisualEffect : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _duration;
    
    public UnityEvent onSetDestination;

    public void SetDestination(Vector3Int hexPosition, Action callback, bool explodeImmediately = true)
    {
        Vector3 worldPosition = Hex.Hex2World(hexPosition);

        StartCoroutine(MoveTo(worldPosition, callback, explodeImmediately));
    }

    private IEnumerator MoveTo(Vector3 targetPosition, Action callback, bool explodeImmediately)
    {
        //포물선을 그리며 targetPosition으로 이동
        const float duration = 1f;

        Vector3 startPosition = transform.position;
        
        int randomRotationDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / duration;

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y = Mathf.Sin(progress * Mathf.PI) * 2;

            transform.position = currentPosition;
            
            transform.Rotate(Vector3.up, randomRotationDirection * 360 * Time.deltaTime);

            yield return null;
        }

        if(explodeImmediately) Explode();
        callback();
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
