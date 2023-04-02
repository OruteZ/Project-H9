using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Acting
{
    
    protected Actor actor;

    protected bool isActFinished;
    public abstract void Execute(Tile target);
    public abstract void OnSelect(Actor a);
}

public class Moving : Acting
{
    private const float OneTileMoveTime = 0.2f;

    public override void Execute(Tile target)
    {
        if (!isActFinished) return;
        IEnumerator MoveCoroutine(IEnumerable<Tile> route)
        {
            const float oneDivTileMoveTime = 1 / 0.2f;

            foreach (var dest in route)
            {
                if (dest.hexTransform.position == actor.hexTransform.position) continue;
            
                var start = Hex.Hex2World(actor.hexTransform.position);
                var end = Hex.Hex2World(dest.hexTransform.position);
                var time = 0f;
                while (time <= OneTileMoveTime)
                {
                    time += Time.deltaTime;
                    actor.transform.position = Vector3.Lerp(start, end, time * oneDivTileMoveTime);
                    yield return null;
                }

                actor.hexTransform.position = dest.hexTransform.position;
            }

            isActFinished = true;
        }
        
        var route = actor.world.FindPath(actor.hexTransform.position, target.hexTransform.position);
        if (route == null) return;
        
        actor.StartCoroutine(MoveCoroutine(route));
    }

    public override void OnSelect(Actor actor)
    {
#if UNITY_EDITOR
        Debug.Log("Selected Moving Mode");
#endif
        //throw new System.NotImplementedException();
        this.actor = actor;
        isActFinished = false;
    }
}

public class Attacking : Acting
{
    public override void Execute(Tile target) {}

    public override void OnSelect(Actor a)
    {
        actor = a;
    }
}