using UnityEngine;

public class Town : TileObject
{
    private int _townIndex;
    
    protected override void SetTile(Tile t)
    {
        Debug.Log("Se tTile call");
        base.SetTile(t);

        t.walkable = true;
        t.visible = true;
        t.rayThroughable = false;
    }

    public override void OnCollision(Unit other)
    {
        Debug.Log($"플레이어 진입 : {_townIndex}번 마을");
    }


    public override string[] GetArgs()
    {
        return new[] { _townIndex.ToString() };
    }

    public override void SetArgs(string[] args)
    {
        if(args.Length != 1) throw new System.Exception();
        
        _townIndex = int.Parse(args[0]);
    }
}
