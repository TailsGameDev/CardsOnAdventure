using System.Collections.Generic;
using UnityEngine;

public class ListAudioHolder : AudioHolder
{
    [SerializeField]
    private List<AudioClip> audioClips = null;

    public override AudioClip GetAudioByName(string fileName)
    {
        AudioClip audioClip;
        audioClip = audioClips.Find(x => x.name == fileName);
        if (audioClip == null)
        {
            Debug.LogError("[ListAudioHolder] audioClip is null", this);
        }
        return audioClip;
    }
}
