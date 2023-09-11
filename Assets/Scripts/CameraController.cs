using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public float normalSpeed;
    public float fastSpeed;
    
    public float movementSpeed;
    public float movementTime;

    public float rotationAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;

    [Space(1)] public float lookAtOffset;

    private void Awake()
    {
        FieldSystem.onCombatAwake.AddListener(() =>
        {
            if (GameManager.instance.CompareState(GameState.Combat))
            {
                LookAtPlayer();
            };
        });
        
        var tsf = transform;
        
        newPosition = tsf.position;
        newRotation = tsf.rotation;
    }
    private void Update()
    {
       HandleMoveEvent();
    }

    private void HandleMoveEvent()
    {
        Vector3 direction = Vector3.zero;

        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        direction.Normalize();
        newPosition += direction * (movementSpeed * Time.deltaTime);
        
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);

        if (Input.GetKey(KeyCode.Q)) newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        if (Input.GetKey(KeyCode.E)) newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    private void LookAtUnit(Unit unit)
    {
        var worldPos = Hex.Hex2World(unit.hexPosition);
        worldPos.y = transform.position.y;
        worldPos.z += 2f;

        newPosition = worldPos;
        //transform.position = worldPos;
    }

    private void LookAtPlayer()
    {
        var player = FieldSystem.unitSystem.GetPlayer();
        if(player is null) Debug.LogError("Player is null");
        
        LookAtUnit(player);
    }
}
