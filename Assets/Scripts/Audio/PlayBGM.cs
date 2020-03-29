using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : AudioManager
{
    [SerializeField]
    private AudioClip clip = null;

    private void Start()
    {
        audioManager.PlayBGM(clip);
    }
}
