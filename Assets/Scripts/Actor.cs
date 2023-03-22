using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Actor : Unit
{
    //한칸을 이동하는데 걸리는 시간입니다.
    public float oneTileMoveTime;
    private void Awake()
    {
        Init();
    }

    private void MoveOnce(Vector3Int destination)
    {
        //이 예외처리는 빌드를 뽑기 전에는 최적화를 위해 제거됨
        if (Hex.Distance(hexTransform.position, destination) != 1)
        {
            Debug.Log("MoveOnce 함수는 한칸을 움직일때만 호출됩니다.");
            return;
        }

        
    }

    //todo : Move Coroutine 함수 완성
    private IEnumerator MoveCoroutine(Vector3 start, Vector3 end)
    {
        var totalTime = Vector3.Distance(start, end) *  oneTileMoveTime / (2 * Hex.Radius);

        while (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            

            yield return null;
        }
    }
}

public enum ActorState
{
    Busy,
    ReadyToAct
}
