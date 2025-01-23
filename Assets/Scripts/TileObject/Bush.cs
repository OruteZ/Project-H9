using UnityEngine;

public class Bush : TileObject
{
    public override string[] GetArgs()
    { return new string[] { }; }
    
    public override void SetArgs(string[] args)
    { return; }

    public override void OnHexCollisionEnter(Unit other)
    {
        if(other is not Player)
            other.meshVisible = false;
        return;
    }
    
    public override void OnHexCollisionExit(Unit other)
    {
        other.meshVisible = true;
        return;
    }
    
    public override void SetUp()
    {
        base.SetUp();
        
        tile.visible = false;
        tile.walkable = true;
        tile.rayThroughable = true;
    }
    
    
    #region VISION & MESHRENDERER
    // =========== multi mesh renderer object이기 때문에 이렇게 처리함 ===========
    [Header("Vision")]
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private bool bushVision;
    
    protected override void InitRenderer()
    {
        // get children mesh renderers
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }
    
    public override bool IsVisible()
    {
        return bushVision;
    }
    
    public override void SetVisible(bool value)
    {
        bushVision = value;
        foreach (MeshRenderer r in meshRenderers)
        {
            r.enabled = value;
        }
    }
    
    #endregion
}