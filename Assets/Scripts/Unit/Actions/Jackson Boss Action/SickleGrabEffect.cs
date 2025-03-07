using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SickleGrabEffect : MonoBehaviour
{
    // �罽�� �� ĭ(ť��) ������
    public GameObject cubePrefab;
    // ť�� ���� ���� (�⺻ 1.0 ����)
    public float cubeSpacing = 1.0f;
    // �罽 Ȯ��/���� �ӵ� (�ʴ� �̵� �Ÿ�)
    public float extendSpeed = 5f;
    
    // ��ü ������ �Ϸ�Ǹ� ȣ��� �׼� (Ȯ�� �� ���� �Ϸ�)
    public Action onFinished;

    // ������ ť����� �����ϴ� ����Ʈ (���� �� Ȱ��)
    private readonly List<GameObject> _chainCubes = new List<GameObject>();

    /// <summary>
    /// ���� ��ġ���� target���� �罽�� Ȯ���ϰ�, ���� �ð� �� ���� ��ġ�� �����մϴ�.
    /// ��ü ������ �Ϸ�Ǹ� OnFinished �׼��� ȣ��˴ϴ�.
    /// �� ��ũ��Ʈ�� ü�� �� ����� ������Ʈ�� �����Ͽ� ����ϼ���.
    /// </summary>
    public void ExtendRetractChain(Vector3 target, Action onFinishAction = null)
    {
        this.onFinished = onFinishAction;
        StartCoroutine(ExtendRetractCoroutine(target));
    }

    private IEnumerator ExtendRetractCoroutine(Vector3 target)
    {
        // ü�� Ȯ��
        yield return StartCoroutine(ExtendChainCoroutine(target));
        // Ȯ�� �� ��� ��� (���ϴ� �ð����� ���� ����)
        yield return new WaitForSeconds(0.2f);
        // ü�� ����
        yield return StartCoroutine(RetractChainCoroutine());
        // ��ü ���� �Ϸ� �� �ݹ� ȣ��
        onFinished?.Invoke();
    }

    /// <summary>
    /// ���� ��ġ���� target���� ü���� Ȯ���ϴ� �ڷ�ƾ
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

            // ť�� ���ݿ� ���� ����
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
    /// ü���� ���� ��ġ�� �����ϴ� �ڷ�ƾ
    /// ������ ť����� ������ ť����� �ϳ��� �����մϴ�.
    /// </summary>
    private IEnumerator RetractChainCoroutine()
    {
        // ����Ʈ�� ������ ť����� ����
        while (_chainCubes.Count > 0)
        {
            GameObject lastCube = _chainCubes[^1];
            _chainCubes.RemoveAt(_chainCubes.Count - 1);
            Destroy(lastCube);
            // ť�� �ϳ� ���� �� ���� ���ű����� �ð� ����
            yield return new WaitForSeconds(cubeSpacing / extendSpeed);
        }
    }
}
