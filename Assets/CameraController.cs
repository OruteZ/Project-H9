using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public float speed;
    // Update is called once per frame
    private void Update()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
        if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector2.right;
        if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
        
        direction.Normalize();

        transform.position += new Vector3(direction.x, 0, direction.y) * (speed * Time.deltaTime);
    }
}
