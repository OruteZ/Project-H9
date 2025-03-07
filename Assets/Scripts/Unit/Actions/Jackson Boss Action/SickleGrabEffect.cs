using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SickleGrabEffect : MonoBehaviour
{
    // 사슬의 한 칸(큐브) 프리팹
    public GameObject cubePrefab;
    // 큐브 간의 간격 (기본 1.0 단위)
    public float cubeSpacing = 1.0f;
    // 사슬 확장/수축 속도 (초당 이동 거리)
    public float extendSpeed = 5f;
    
    // 전체 동작이 완료되면 호출될 액션 (확장 후 수축 완료)
    public Action onFinished;

    // 생성된 큐브들을 저장하는 리스트 (수축 시 활용)
    private readonly List<GameObject> _chainCubes = new List<GameObject>();

    /// <summary>
    /// 현재 위치에서 target까지 사슬을 확장하고, 일정 시간 후 원래 위치로 수축합니다.
    /// 전체 동작이 완료되면 OnFinished 액션이 호출됩니다.
    /// 이 스크립트는 체인 낫 모양의 오브젝트에 부착하여 사용하세요.
    /// </summary>
    public void ExtendRetractChain(Vector3 target, Action onFinishAction = null)
    {
        this.onFinished = onFinishAction;
        StartCoroutine(ExtendRetractCoroutine(target));
    }

    private IEnumerator ExtendRetractCoroutine(Vector3 target)
    {
        // 체인 확장
        yield return StartCoroutine(ExtendChainCoroutine(target));
        // 확장 후 잠시 대기 (원하는 시간으로 조정 가능)
        yield return new WaitForSeconds(0.2f);
        // 체인 수축
        yield return StartCoroutine(RetractChainCoroutine());
        // 전체 동작 완료 후 콜백 호출
        onFinished?.Invoke();
    }

    /// <summary>
    /// 시작 위치에서 target까지 체인을 확장하는 코루틴
    /// </summary>
    private IEnumerator ExtendChainCoroutine(Vector3 target)
    {
        Vector3 start = transform.position;
        Vector3 direction = (target - start).normalized;
        float totalDistance = Vector3.Distance(start, target);
        float currentDistance = 0f;
        int cubeIndex = 0;

        while (currentDistance < totalDistance)
        {
            float step = extendSpeed * Time.deltaTime;
            currentDistance += step;
            if (currentDistance > totalDistance)
                currentDistance = totalDistance;

            // 큐브 간격에 맞춰 생성
            float nextCubeDistance = cubeIndex * cubeSpacing;
            while (nextCubeDistance < currentDistance && nextCubeDistance <= totalDistance)
            {
                Vector3 cubePosition = start + direction * nextCubeDistance;
                GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.LookRotation(direction), transform);
                _chainCubes.Add(cube);
                cubeIndex++;
                nextCubeDistance = cubeIndex * cubeSpacing;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 체인을 원래 위치로 수축하는 코루틴
    /// 생성된 큐브들을 마지막 큐브부터 하나씩 제거합니다.
    /// </summary>
    private IEnumerator RetractChainCoroutine()
    {
        // 리스트의 마지막 큐브부터 제거
        while (_chainCubes.Count > 0)
        {
            GameObject lastCube = _chainCubes[^1];
            _chainCubes.RemoveAt(_chainCubes.Count - 1);
            Destroy(lastCube);
            // 큐브 하나 제거 후 다음 제거까지의 시간 간격
            yield return new WaitForSeconds(cubeSpacing / extendSpeed);
        }
    }
}
