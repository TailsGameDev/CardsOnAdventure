using UnityEngine;
using UnityEngine.UI;

public class StorySpeech : MonoBehaviour
{
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Text speechText = null;
    [SerializeField] private Image background = null;

    private TransformWrapper transformWrapper;

    public void Initialize(Sprite characterSprite, string speechMessage, Color backgroundColor, TransformWrapper newParent)
    {
        this.characterImage.sprite = characterSprite;
        this.speechText.text = speechMessage;
        this.background.color = backgroundColor;

        transformWrapper = new TransformWrapper(transform);

        transformWrapper.SetParent(newParent);

        SetGameObjectActive(false);
    }

    public void SetGameObjectActive(bool activate)
    {
        gameObject.SetActive(activate);
    }
}
