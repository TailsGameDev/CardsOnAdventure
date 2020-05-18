using System;
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

    [SerializeField]
    private Text currentValueMusic = null;
    [SerializeField]
    private Text currentValueSFX = null;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
        }

        // Destroy handled by DontDestroyOnLoadCanvas.cs

        if (PlayerPrefs.HasKey(BGM_VOLUME_KEY))
        {
            BGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
            MusicSlider.value = BGMSource.volume * 4;
            UpdateCurrentValueText(currentValueMusic, MusicSlider);
        }

        if (PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            foreach (AudioSource source in SFXSources)
            {
                source.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
            }
            SFXSlider.value = SFXSources[0].volume * 4;
            UpdateCurrentValueText(currentValueSFX, SFXSlider);
        }
    }

    private void UpdateCurrentValueText(Text text, Slider slider)
    {
        int intermediate = (int)(slider.value * 100);
        float v = intermediate; // ((float)intermediate) / 10;
        text.text = v.ToString();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != BGMSource.clip)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
    }

    public void StopBGM()
    {
        BGMSource.Stop();
    }
    public void SetBGMClipToNull()
    {
        BGMSource.clip = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource sfxSource = GetFreeSFXAudioSource();
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopAllSFX()
    {
        foreach(AudioSource source in SFXSources)
        {
            source.Stop();
        }
    }

    public void PlayOneOfClipsWithRandomizedPitch(params AudioClip[] clips)
    {
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);
        float randomPitch = UnityEngine.Random.Range(LowPitchRange, HighPitchRange);

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
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BGMSource.volume);
        UpdateCurrentValueText(currentValueMusic, MusicSlider);
    }

    public void OnSFXSliderValueChanged()
    {
        foreach (AudioSource source in SFXSources)
        {
            source.volume = SFXSlider.value/4;
        }
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFXSources[0].volume);
        UpdateCurrentValueText(currentValueSFX, SFXSlider);
    }
}