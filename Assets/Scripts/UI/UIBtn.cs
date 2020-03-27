using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtn : MonoBehaviour
{
    [SerializeField]
    private SoundRequisitor soundRequisitor = null;

    [SerializeField]
    private AudioClip btnClickSound = null;

    [SerializeField]
    private Image imageComponent = null;

    private RectTransform rectTransform = null;

    [SerializeField]
    private Sprite normalSprite = null;

    [SerializeField]
    private Sprite clickedSprite = null;

    private Vector3 normalSize = new Vector3(1.0f, 1.0f, 1.0f);

    [SerializeField]
    private Vector3 bigSize;

    [SerializeField]
    private Text textComponent = null;

    [SerializeField]
    private float textVerticalDesloc;

    private float originalRectTransfmOffsetMaxDotY;

    [SerializeField]
    private Color BtnDownTextColor;

    private Color BtnUpTextColor;

    protected void Awake()
    {
        rectTransform = imageComponent.GetComponent<RectTransform>();
        // TODO: 'rectTransform.offsetMax.y;' or somehing should be better but didn't work at the first attempt
        originalRectTransfmOffsetMaxDotY = 0;
        BtnUpTextColor = textComponent.color;
    }

    public void OnPointerEntered()
    {
        rectTransform.localScale = bigSize;
    }

    public void OnPointerExited()
    {
        rectTransform.localScale = normalSize;
    }

    public void UpToDownBtnVisualAndSoundEffects()
    {
        ConfigureBtnLooks(clickedSprite, textVerticalDesloc, BtnDownTextColor);
        soundRequisitor.MakeSFXRequest(btnClickSound);
    }

    public void DownToUpBtnVisualAndSoundEffects()
    {
        ConfigureBtnLooks(normalSprite, originalRectTransfmOffsetMaxDotY, BtnUpTextColor);
    }

    private void ConfigureBtnLooks(Sprite sprite, float top, Color color)
    {
        imageComponent.sprite = sprite;
        SetTop(top);
        textComponent.color = color;
    }

    public void SetTop(float top)
    {
        RectTransform rt = textComponent.GetComponent<RectTransform>(); 
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
}
