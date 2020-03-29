using System.Collections;
using UnityEngine;

public abstract class AudioHolder : MonoBehaviour
{
    public abstract AudioClip GetAudioByName(string fileName);

    public AudioClip GetAleatoryClipAmong(IEnumerable fileNames)
    {
        string[] names = (string[])fileNames;

        int randomIndex = UnityEngine.Random.Range(0, names.Length);

        return GetAudioByName(names[randomIndex]);
    }
}
