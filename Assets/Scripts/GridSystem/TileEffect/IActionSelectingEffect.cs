public interface IActionSelectingEffect
{
    /// <summary>
    /// Stop All Effect
    /// </summary>
    void StopEffect();
    
    /// <summary>
    /// Show User's Tile Effect
    /// </summary>
    /// <param name="user"></param>
    void ShowTileEffect(Unit user);

    /// <summary>
    /// Get ActionType that this effect has
    /// </summary>
    /// <param name="setting"></param>
    ActionType GetActionType();
    
    // initialize
    void SetupTileEffect(TileEffectSetting setting);
}

// ReSharper disable once InvalidXmlDocComment
/// <docs>
/// Tile Effect can divide into 2 kind
/// 1. Base Effect : When Select Actions, this Effect will be set at once
/// 2. Dynamic Effect : This Effect Will be changed depends on player's mouse pointer
/// <\docs>