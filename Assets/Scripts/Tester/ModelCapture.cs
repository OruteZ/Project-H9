using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class ModelCapture : MonoBehaviour
{
    public string objPath;
    public string pngPath;

    private GameObject _obj;
    
    private readonly Queue<GameObject> _objQueue = new Queue<GameObject>();

    public void Capture()
    {
        //get all obj in resources
        var obj = Resources.LoadAll<GameObject>(objPath);
        
        //enqueue all obj
        foreach (var o in obj)
        {
            _objQueue.Enqueue(o);
        }
    }

    public void Update()
    {
        //if _obj is instantiated, remove it
        if (_obj != null)
        {
            Destroy(_obj);
            _obj = null;
            return;
        }

        //if queue si empty return
        if (_objQueue.Count == 0) return;
        
        //dequeue obj
        var obj = _objQueue.Dequeue();
            
        //instant obj
        _obj = Instantiate(obj, null, true);
            
        //set obj active
        _obj.SetActive(true);
            
        //replace obj name : (Clone) to empty string
        _obj.name = obj.name.Replace("(Clone)", "");
        
        //obj look at camera and lock y axis
        Debug.Assert(Camera.main != null, "Camera.main != null");
        
        _obj.transform.position = new Vector3(0, 0, 0);
        _obj.transform.localScale = new Vector3(1, 1, 1);

        _obj.transform.LookAt(Camera.main.transform);
        _obj.transform.eulerAngles = new Vector3(0, _obj.transform.eulerAngles.y, 0);   

        //capture obj
        ScreenCapture.CaptureScreenshot(pngPath +'/'+ obj.name + ".png");
    }
}