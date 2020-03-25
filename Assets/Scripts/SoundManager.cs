using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource SFXSource;

    public Slider MusicSlider;
    public Slider SFXSlider;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void PlayOneOfClipsWithRandomizedPitch(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        SFXSource.pitch = randomPitch;
        SFXSource.clip = clips[randomIndex];
        SFXSource.Play();
    }

    public void OnBGMSliderValueChanged()
    {
        BGMSource.volume = MusicSlider.value;
    }

    public void OnSFXSliderValueChanged()
    {
        SFXSource.volume = SFXSlider.value;
    }
}