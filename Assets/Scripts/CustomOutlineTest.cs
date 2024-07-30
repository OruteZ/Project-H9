using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOutlineTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject clickedObject;
            if (IsMouseClickedOutlineObject(out clickedObject))
            {
                CustomOutline outline = null;
                if (clickedObject.TryGetComponent(out outline))
                {
                    outline.enabled = !outline.enabled;
                }
            }
        }
    }
    private static bool IsMouseClickedOutlineObject(out GameObject clickedObject)
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isSuccessRaycast = Physics.Raycast(ray, out var hit, float.MaxValue);
            if (isSuccessRaycast)
            {
                GameObject obj = hit.collider.gameObject;
                clickedObject = obj;
                return true;
            }
        }

        clickedObject = null;
        return false;
    }
}
