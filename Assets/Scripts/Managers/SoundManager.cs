using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSource;
    private List<AudioSource> _sfxSources;

    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public bool isMute = false;

    protected override void Awake()
    {
        base.Awake();
        
        bgmSource = gameObject.AddComponent<AudioSource>();
        _sfxSources = new List<AudioSource>();
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
    
    public void PlayBGM(string name)
    {
        for (int i = 0; i < bgmClips.Length; i++)
        {
            if (bgmClips[i].name != name) continue;
            
            PlayBGM(i);
            return;
        }
        Debug.LogError("BGM " + name + " not found");
    }

    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        //null check
        if (clip == null) return;
        
        // 임시로 해당 위치에 효과음을 재생하는 코드, 차후 게임오브젝트를 직접 생성해서 만들어내고 
        // 오브젝트 풀링을 통해 관리하는 방식으로 변경해야 함
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void Mute()
    {
        isMute = !isMute;
        bgmSource.mute = isMute;
        // sfxSource.mute = isMute;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        // sfxSource.volume = sfxVolume;
    }
}