using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMadeSoundRequest
{
    private enum SoundType
    {
        BGM,
        SFX
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

    public void RequestPlaying()
    {
        if(soundType == SoundType.BGM)
        {
            requisitor.RequestBGM(audioClip);
        } 
        else
        {
            requisitor.RequestSFX(audioClip);
        }
    }

}
