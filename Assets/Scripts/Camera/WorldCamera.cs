using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldCamera : MonoBehaviour
{
    private Vector3 _lastMousePosition;
    
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;

    public WorldCamera(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }
    
    private void Update()
    {
        HandleMoveEvent();
    }
    private void HandleMoveEvent()
    {
        // 마우스 드래그에 의한 이동, X와 Z값만을 변경합니다.
        // if (Input.GetMouseButtonDown(0))
        // {
        //     _lastMousePosition = Input.mousePosition;
        // }
        // if (Input.GetMouseButton(0))
        // {
        //     Vector3 delta = Input.mousePosition - _lastMousePosition;
        //     transform.Translate(-delta.x * (movementSpeed * Time.deltaTime), 0, -delta.y * (movementSpeed * Time.deltaTime));
        //     _lastMousePosition = Input.mousePosition;
        // }

        // WASD 키 입력에 의한 이동
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        transform.Translate(direction * (movementSpeed * Time.deltaTime));

        // 화면 가장자리에 마우스 커서에 의한 이동
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x < 0) transform.Translate(Vector3.left * (movementSpeed * Time.deltaTime));
        if (mousePosition.x > Screen.width) transform.Translate(Vector3.right * (movementSpeed * Time.deltaTime));
        if (mousePosition.y < 0) transform.Translate(Vector3.back * (movementSpeed * Time.deltaTime));
        if (mousePosition.y > Screen.height) transform.Translate(Vector3.forward * (movementSpeed * Time.deltaTime));
        
        // 이동 제한 적용
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);
        //clamp y value to 0
        clampedPosition.y = 0;
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minPosition.z, maxPosition.z);
        transform.position = clampedPosition;
    }
    
    #region EDITOR
    [ContextMenu("SetMinPosition")]
    private void SetMinPosition()
    {
        minPosition = transform.position;
    }
    
    [ContextMenu("SetMaxPosition")]
    private void SetMaxPosition()
    {
        maxPosition = transform.position;
    }
    #endregion

    public void SetPriority(int i)
    {
        virtualCamera.Priority = i;
    }
    
    public void SetPosition(Vector3 position)
    {
        Debug.Log("SetPosition : " + position);
        
        //ignore y value
        position.y = 0;
        
        transform.position = position;
    }
}