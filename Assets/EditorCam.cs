using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCam : MonoBehaviour
{
    public float speed;
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        
        direction.Normalize();

        transform.position += direction * (speed * Time.deltaTime);
    }
}
