using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource general;
    public AudioSource music;
    public AudioSource ambience;
    public AudioSource ui;

    AudioClip musicClip;

    public void PlayAudio(AudioSource audioSource, bool loop)
    {
        audioSource.loop = loop;
        audioSource.Play();
    }
    
    public void PlayAudio(AudioClip clip, AudioSource audioSource, bool loop)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void PauseAudio(AudioSource audioSource)
    {        
        audioSource.Pause();
    }

    public void MuteAllAudio()
    {
        general.Pause();
        music.Pause();
        ambience.Pause();
        ui.Pause();
    }

    void Start()
    {
        musicClip = Resources.Load<AudioClip>("Audio/Music/SaveMe");
        music.clip = musicClip;
    }
}
