using System;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryAudioHolder : AudioHolder
{
    [SerializeField]
    private AudioClip silence = null;

    [Serializable]
    private struct DictItem
    {
        #pragma warning disable 649
        public string Key;
        public AudioClip Value;
        #pragma warning restore 649
    }

    [SerializeField]
    private List<DictItem> myDictionary = null;

    public override AudioClip GetAudioByName (string fileName) {

        AudioClip audioClip = silence;

        audioClip = myDictionary.Find(name => name.Key == fileName).Value;

        Debug.LogWarning(audioClip.name);

        if (audioClip == silence)
        {
            Debug.LogError("[AudioHolder] Audio named "+fileName+" was not found in audioCollection", this);
        }

        return audioClip;
    }
}
