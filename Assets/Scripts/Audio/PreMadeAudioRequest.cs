using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMadeAudioRequest
{
    private enum SoundType
    {
        BGM,
        SFX,
        SFX_AND_STOP_BGM,
        STOP_SFX,
    }

    private AudioClip audioClip;
    private AudioRequisitor requisitor;
    private SoundType soundType;
    private GameObject assignor;

    private PreMadeAudioRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor, SoundType soundType)
    {
        this.audioClip = audioClip;
        this.requisitor = requisitor;
        this.soundType = soundType;
        this.assignor = assignor;
    }

    public static PreMadeAudioRequest CreateBGMSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClip, requisitor, assignor, SoundType.BGM);
    }

    public static PreMadeAudioRequest CreateSFXSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClip, requisitor,assignor, SoundType.SFX);
    }

    public static PreMadeAudioRequest CreateSFX_AND_STOP_BGMSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(audioClip, requisitor, assignor, SoundType.SFX_AND_STOP_BGM);
    }

    public static PreMadeAudioRequest CreateSTOP_SFXAudioRequest(AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeAudioRequest(null, requisitor, assignor, SoundType.STOP_SFX);
    }

    public void RequestPlaying()
    {
        switch (soundType)
        {
            case SoundType.BGM:
                requisitor.RequestBGM(audioClip);
                break;
            case SoundType.SFX:
                requisitor.RequestSFX(audioClip);
                break;
            case SoundType.SFX_AND_STOP_BGM:
                requisitor.RequestSFX(audioClip);
                requisitor.RequestStopBGM();
                break;
            case SoundType.STOP_SFX:
                requisitor.RequestStopSFX();
                break;
        }
    }

}
