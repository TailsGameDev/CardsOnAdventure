using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{

    [SerializeField]
    private AudioClip clip = null;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(clip);
    }
}
