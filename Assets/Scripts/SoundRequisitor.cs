using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRequisitor : SoundManager
{
    public void MakeBGMRequest(AudioClip audioClip)
    {
        soundManager.PlayBGM(audioClip);
    }

    public void MakeSFXRequest(AudioClip audioClip)
    {
        soundManager.PlaySFX(audioClip);
    }
}
