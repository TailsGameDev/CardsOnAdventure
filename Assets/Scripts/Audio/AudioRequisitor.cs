using UnityEngine;

public class AudioRequisitor : AudioManager
{
    public void RequestBGM(AudioClip audioClip)
    {
        audioManager.PlayBGM(audioClip);
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
