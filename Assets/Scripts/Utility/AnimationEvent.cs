public static class AnimationEventNames
{
    public const string GUN_FIRE = "GunFire";
    public const string GUN_RELOAD = "GunReload";
    public const string GUN_RELOAD_END = "GunReloadEnd";
    public const string STEP = "Step";
    public const string THROW = "Throw";
    public const string COVER = "Cover";
    
    // unit model's animation event name
    public const string COVER_START = "CoverStart";
    public const string COVER_END = "CoverEnd";

    public static bool IsEventName(string eventName)
    {
        return eventName
            is GUN_FIRE
            or GUN_RELOAD
            or GUN_RELOAD_END
            or STEP
            or THROW
            or COVER
            or COVER_START
            or COVER_END
            
       ;
    }
}