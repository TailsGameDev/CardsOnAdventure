using UnityEngine;
using UnityEngine.UI;

public class StorySpeech : MonoBehaviour
{
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Text speechText = null;
    [SerializeField] private Image background = null;

    private Transform transformWrapper;

    public void Initialize(Sprite characterSprite, string speechMessage, Color backgroundColor, Transform newParent)
    {
        this.characterImage.sprite = characterSprite;
        this.speechText.text = speechMessage;
        this.background.color = backgroundColor;

        transformWrapper = transform;

        transformWrapper.SetParent(newParent);

        SetGameObjectActive(false);
    }

    public void SetGameObjectActive(bool activate)
    {
        gameObject.SetActive(activate);
    }
}
