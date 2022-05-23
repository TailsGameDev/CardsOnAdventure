using UnityEngine;
using UnityEngine.UI;

public class StorySpeech : MonoBehaviour
{
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Text speechText = null;
    [SerializeField] private Image background = null;

    public void Initialize(Sprite characterSprite, string speechMessage, Color backgroundColor)
    {
        this.characterImage.sprite = characterSprite;
        this.speechText.text = speechMessage;
        this.background.color = backgroundColor;
    }
}
