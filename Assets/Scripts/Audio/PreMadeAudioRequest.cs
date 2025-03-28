﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMadeAudioRequest
{
    private enum SoundType
    {
        BGM_LOOP,
        SFX,
        SFX_AND_STOP_BGM_AND_SET_BGMCLIP_TO_NULL,
        STOP_SFX,
        BGM_ONE_SHOT,
    }

    private AudioClip[] audioClips;
    private AudioRequisitor requisitor;
    private SoundType soundType;
    private GameObject assignor;

    private PreMadeAudioRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor, SoundType soundType)
    {
        this.audioClips = new AudioClip[1] { audioClip };
        this.requisitor = requisitor;
        this.soundType = soundType;
        this.assignor = assignor;
    }
    private PreMadeAudioRequest(AudioClip[] audioClips, AudioRequisitor requisitor, GameObject assignor, SoundType soundType)
    {
        this.audioClips = audioClips;
        this.requisitor = requisitor;
        this.soundType = soundType;
        this.assignor = assignor;
    }

    public static PreMadeAudioRequest CreateBGMOneShotAudioRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClip, requisitor, assignor, SoundType.BGM_LOOP);
    }
    public static PreMadeAudioRequest CreateSFXSoundRequest(AudioClip[] audioClips, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClips, requisitor,assignor, SoundType.SFX);
    }
    public static PreMadeAudioRequest CreateSFXSoundRequest(AudioClip audioClips, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClips, requisitor, assignor, SoundType.SFX);
    }
    public static PreMadeAudioRequest CreateSFX_AND_STOP_BGMAudioRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClip, requisitor, assignor, SoundType.SFX_AND_STOP_BGM_AND_SET_BGMCLIP_TO_NULL);
    }
    public static PreMadeAudioRequest CreateSTOP_SFXAudioRequest(AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(new AudioClip[] { }, requisitor, assignor, SoundType.STOP_SFX);
    }
    public void RequestPlaying()
    {
        int r = Random.Range(0,audioClips.Length);

        switch (soundType)
        {
            case SoundType.BGM_LOOP:
                requisitor.RequestBGMAndLoop(audioClips[r]);
                break;
            case SoundType.SFX:
                requisitor.RequestSFX(audioClips[r]);
                break;
            case SoundType.SFX_AND_STOP_BGM_AND_SET_BGMCLIP_TO_NULL:
                requisitor.RequestSFX(audioClips[r]);
                requisitor.RequestStopBGMAndSetBGMClipToNull();
                break;
            case SoundType.STOP_SFX:
                requisitor.RequestStopSFX();
                break;
            case SoundType.BGM_ONE_SHOT:
                requisitor.RequestBGMToPlayOneSingleTime(audioClips[r]);
                break;
        }
    }
}
