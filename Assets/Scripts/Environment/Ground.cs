using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : EnvObject
{
    public Mesh mesh1;
    public Mesh mesh2;
    
    public override void OnSetting()
    {
        GetComponent<MeshFilter>().mesh = Random.value > 0.5f ? mesh1 : mesh2;
        RotateRandom();
    }
}
