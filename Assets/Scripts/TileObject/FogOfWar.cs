using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class FogOfWar : TileObject
{
    private bool _isRemoving = false;
    public float removingDuration;

    private const float TO_CAM = 0.5f;

    public new void SetVisible(bool value)
    {
        if (value)
        {
            GameManager.instance.AddPioneeredWorldTile(hexPosition);
            RemoveSelf();
        }

        else
        {
            //시야에 안들어오면 얘는 반대로 보여야됨
            base.SetVisible(true);
        }
    }

    protected override void RemoveSelf()
    {
        if (_isRemoving) return;
        
        
        FieldSystem.tileSystem.DeleteTileObject(this);
        tile.RemoveObject(this);
        
        _isRemoving = true;
        StartCoroutine(RemovingCoroutine());
    }

    public override string[] GetArgs()
    {
        return null;
    }

    public override void SetArgs(string[] args)
    {
        Debug.LogError("Try to set arguments in fow of war");
        throw new System.Exception();
    }

    private IEnumerator RemovingCoroutine()
    {
        float durationReciprocal = 1 / removingDuration; 
        float t = 0;

        var originalScale = transform.localScale;
        
        while (t < removingDuration)
        {
            yield return null;
            
            t += Time.deltaTime;
            var percentage = t * durationReciprocal;
            gameObject.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, percentage);
        }
        
        Destroy(gameObject);
    }
}
