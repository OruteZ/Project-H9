using Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public bool isMute = false;

    protected override void Awake()
    {
        base.Awake();
        bgmSource = gameObject.GetOrAddComponent<AudioSource>();
        sfxSource = gameObject.GetOrAddComponent<AudioSource>();
    }

    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogError("BGM index out of range");
            return;
        }

        bgmSource.clip = bgmClips[index];
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length)
        {
            Debug.LogError("SFX index out of range");
            return;
        }

        sfxSource.PlayOneShot(sfxClips[index], sfxVolume);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void Mute()
    {
        isMute = !isMute;
        bgmSource.mute = isMute;
        sfxSource.mute = isMute;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }
}