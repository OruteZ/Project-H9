using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class FogOfWar : TileObject
{
    private bool _isRemoving = false;
    public float removingDuration;
    public new void SetVisible(bool value)
    {
        if (value)
        {
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

        tile.RemoveObject(this);
        _isRemoving = true;
        StartCoroutine(RemovingCoroutine());
    }

    private IEnumerator RemovingCoroutine()
    {
        float durationReciprocal = 1 / removingDuration; 
        float t = 0;

        var originalScale = meshRenderer.transform.localScale;
        
        while (t < removingDuration)
        {
            yield return null;
            
            t += Time.deltaTime;
            var percentage = t * durationReciprocal;
            meshRenderer.gameObject.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, percentage);
        }
        
        Destroy(gameObject);
    }
}
