using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPVolumeChanger : TileObject
{
    [SerializeField] private StageStyle from;
    [SerializeField] private StageStyle to;
    [SerializeField] private float duration = 1f;
    
    public override string[] GetArgs()
    {
        return new[]
        {
            from.ToString(),
            to.ToString(),
            duration.ToString()
        };
    }

    public override void SetArgs(string[] args)
    {
        if (args.Length != 3) throw new System.Exception("Invalid args length");
        
        from = (StageStyle) System.Enum.Parse(typeof(StageStyle), args[0]);
        to = (StageStyle) System.Enum.Parse(typeof(StageStyle), args[1]);
        duration = float.Parse(args[2]);
    }
    
    public override void OnCollision(Unit other)
    {
        StartCoroutine(ChangePPVolume());
    }

    private IEnumerator ChangePPVolume()
    {
        var lightingManager = LightingManager.instance;
        
        float elapsedTime = 0;
        float duration = 1f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            lightingManager.LerpChange(from, to, elapsedTime / duration);
            yield return null;
        }
        
        lightingManager.LerpChange(from, to, 1);
    }
}