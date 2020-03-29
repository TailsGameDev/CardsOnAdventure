using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheOnlyAudioManagerInstance : AudioManager
{
    public AudioSource BGMSource;
    public AudioSource[] SFXSources;

    public Slider MusicSlider;
    public Slider SFXSlider;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
        }
        // Destroy handled by DontDestroyOnLoadCanvas.cs
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != BGMSource.clip)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource SFXSource = GetFreeSFXAudioSource();

        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void PlayOneOfClipsWithRandomizedPitch(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        AudioSource SFXSource = GetFreeSFXAudioSource();

        SFXSource.pitch = randomPitch;
        SFXSource.clip = clips[randomIndex];
        SFXSource.Play();
    }

    private AudioSource GetFreeSFXAudioSource()
    {
        AudioSource audioSource = SFXSources[0];
        foreach (AudioSource source in SFXSources)
        {
            if ( !source.isPlaying )
            {
                audioSource = source;
                break;
            }
        }
        return audioSource;
    }

    public void OnBGMSliderValueChanged()
    {
        BGMSource.volume = MusicSlider.value/4;
    }

    public void OnSFXSliderValueChanged()
    {
        foreach (AudioSource source in SFXSources)
        {
            source.volume = SFXSlider.value/4;
        }
    }
}