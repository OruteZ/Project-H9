using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI.Extensions;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSource;
    private List<AudioSource> _sfxSources;

    [FormerlySerializedAs("bgmClips")] public AudioClip[] worldBgmClips;
    public AudioClip[] combatBgmClips;

    public float mainVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public bool isMute = false;

    protected override void Awake()
    {
        base.Awake();
        
        //if this is null, return
        if (this == null) return;
        
        bgmSource = gameObject.AddComponent<AudioSource>();
        _sfxSources = new List<AudioSource>();
        
        FieldSystem.onStageAwake.AddListener(() =>
        {
            if(GameManager.instance.CompareState(GameState.WORLD))
                PlayBGM(worldBgmClips, Random.Range(0, worldBgmClips.Length));
            
            else 
                PlayBGM(combatBgmClips, Random.Range(0, combatBgmClips.Length));
        });
    }

    private void Update()
    {
        // if bgm play is at finish line, play next bgm
        if (!bgmSource.isPlaying)
        {
            if(GameManager.instance.CompareState(GameState.WORLD))
                PlayBGM(worldBgmClips, Random.Range(0, worldBgmClips.Length));
            
            else 
                PlayBGM(combatBgmClips, Random.Range(0, combatBgmClips.Length));
        }
    }

    public void PlayBGM(AudioClip[] clips, int index)
    {
        if (index < 0 || index >= worldBgmClips.Length)
        {
            Debug.LogError("BGM index out of range");
            return;
        }
        

        bgmSource.clip = clips[index];
        bgmSource.volume = mainVolume * bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    
    public void PlayBGM(AudioClip[] clips, string name)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name != name) continue;
            
            PlayBGM(clips, i);
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
        AudioSource.PlayClipAtPoint(clip, position, mainVolume * sfxVolume);
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

    public void SetMainVolume(float volume)
    {
        mainVolume = volume;
        bgmSource.volume = mainVolume * bgmVolume;
    }
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = mainVolume * bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        // sfxSource.volume = mainVolume * sfxVolume;
    }
}