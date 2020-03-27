using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : SoundManager
{
    [SerializeField]
    private AudioClip clip = null;

    private void Start()
    {
        soundManager.PlayBGM(clip);
    }
}
