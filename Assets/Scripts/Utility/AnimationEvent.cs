public static class AnimationEventNames
{
    public const string GUN_FIRE = "GunFire";
    public const string GUN_RELOAD = "GunReload";
    public const string GUN_RELOAD_END = "GunReloadEnd";
    public const string STEP = "Step";

    public static bool IsEventName(string eventName)
    {
        return eventName 
            is GUN_FIRE 
            or GUN_RELOAD 
            or GUN_RELOAD_END
            
       ;
    }
}