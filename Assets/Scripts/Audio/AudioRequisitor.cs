using UnityEngine;

public class AudioRequisitor : AudioManager
{
    public void RequestBGMAndLoop(AudioClip audioClip)
    {
        audioManager.PlayBGMLoop(audioClip);
    }
    public void RequestBGMToPlayOneSingleTime(AudioClip audioClip)
    {
        audioManager.PlayBGMOneSingleTime(audioClip);
    }

    public void RequestSFX(AudioClip audioClip)
    {
        audioManager.PlaySFX(audioClip);
    }

    public void RequestStopBGMAndSetBGMClipToNull()
    {
        audioManager.StopBGM();
        audioManager.SetBGMClipToNull();
    }

    public void RequestStopSFX()
    {
        audioManager.StopAllSFX();
    }
}
