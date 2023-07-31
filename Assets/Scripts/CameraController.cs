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

    private void Start()
    {
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

    
}
