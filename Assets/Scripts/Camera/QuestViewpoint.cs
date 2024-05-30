using System.Collections;
using Cinemachine;
using UnityEngine;

public class QuestViewpoint : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    
    [SerializeField]
    private int _priority;
    
    [SerializeField]
    private float duration;

    public void SetPosition(Vector3Int hexPos)
    {
        Vector3 worldPos = Hex.Hex2World(hexPos);
        worldPos.y = 0;
        transform.position = worldPos;
    }

    public void View(float dur)
    {
        if (dur == -1) 
            dur = this.duration;
        
        _virtualCamera.Priority = _priority;
        
        StartCoroutine(StopView());
    }

    private IEnumerator StopView()
    {
        yield return new WaitForSeconds(duration);
        
        _virtualCamera.Priority = 0;
    }
}