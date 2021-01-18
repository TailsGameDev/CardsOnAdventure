using UnityEngine;

public class PlaySFX : AudioManager
{
    [SerializeField]
    private AudioClip clip = null;

    private void Start()
    {
        audioManager.PlaySFX(clip);
    }
}
