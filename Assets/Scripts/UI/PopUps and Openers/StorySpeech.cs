using UnityEngine;
using UnityEngine.UI;

public class StorySpeech : MonoBehaviour
{
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Text speechText = null;
    [SerializeField] private Image background = null;


    public void Initialize(Sprite characterSprite, string speechMessage, Color backgroundColor, Transform newParent)
    {
        this.characterImage.sprite = characterSprite;
        this.speechText.text = speechMessage;
        this.background.color = backgroundColor;

        transform.SetParent(newParent);
        transform.localScale = Vector3.one;

        SetGameObjectActive(false);
    }

    public void SetGameObjectActive(bool activate)
    {
        gameObject.SetActive(activate);
    }
}
