using UnityEngine;

public class UnitSFXEffector : MonoBehaviour
{
    public AudioClip gunFireClip;
    public AudioClip gunReloadClip;
    public AudioClip gunReloadEndClip;
    public AudioClip stepClip;
    
    public void GetAnimationEvent(string eventName)
    {;
        if (!AnimationEventNames.IsEventName(eventName))
        {
            Debug.LogError("Invalid event name: " + eventName);
            return;
        }
        
        var targetClip = eventName switch
        {
            AnimationEventNames.GUN_FIRE => gunFireClip,
            AnimationEventNames.GUN_RELOAD => gunReloadClip,
            AnimationEventNames.GUN_RELOAD_END => gunReloadEndClip,
            AnimationEventNames.STEP => stepClip,
            _ => null
        };
        if (targetClip is null) return;
        
        Debug.Log("SOUND_DEBUG: " + eventName + " is called.");
        SoundManager.instance.PlaySFX(targetClip, transform.position);
    }
}