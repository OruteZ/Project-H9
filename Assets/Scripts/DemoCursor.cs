using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCursor: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        
        if (Input.anyKeyDown)
        {
            Big = true;
            Invoke(nameof(GetCursorSmall), 0.1f);
        }
    }

    private void GetCursorSmall()
    {
        Big = false;
    }

    private bool _big;
    private bool Big
    {
        get => _big;
        set
        {
            if (value == _big) return;
            transform.localScale *= value ? 2 : 0.5f;
            _big = value;
        }
    }
}
