using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMadeSoundRequest
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

    private PreMadeSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor, SoundType soundType)
    {
        this.audioClip = audioClip;
        this.requisitor = requisitor;
        this.soundType = soundType;
        this.assignor = assignor;
    }

    public static PreMadeSoundRequest CreateBGMSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeSoundRequest(audioClip, requisitor, assignor, SoundType.BGM);
    }

    public static PreMadeSoundRequest CreateSFXSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeSoundRequest(audioClip, requisitor,assignor, SoundType.SFX);
    }

    public static PreMadeSoundRequest CreateSFX_AND_STOP_BGMSoundRequest(AudioClip audioClip, AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeSoundRequest(audioClip, requisitor, assignor, SoundType.SFX_AND_STOP_BGM);
    }

    public static PreMadeSoundRequest CreateSTOP_SFXSoundRequest(AudioRequisitor requisitor, GameObject assignor)
    {
        return new PreMadeSoundRequest(null, requisitor, assignor, SoundType.STOP_SFX);
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
