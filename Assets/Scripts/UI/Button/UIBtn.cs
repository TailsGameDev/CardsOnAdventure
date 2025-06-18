using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtn : MonoBehaviour
{
    [SerializeField]
    private AudioRequisitor soundRequisitor = null;

    [SerializeField]
    private AudioClip btnClickSound = null;

    [SerializeField]
    private Image imageComponent = null;

    [SerializeField]
    private RectTransform rectTransform = null;

    [SerializeField]
    protected Sprite normalSprite = null;

    [SerializeField]
    private Sprite clickedSprite = null;

    private Vector3 normalSize = new Vector3(1.0f, 1.0f, 1.0f);

    [SerializeField]
    private Vector3 bigSize = Vector3.zero;

    [SerializeField]
    private TextMeshProUGUI textComponent = null;

    [SerializeField]
    private RectTransform rectTransformToGoDownWhileBtnPressed = null;

    [SerializeField]
    private float textVerticalDesloc = 0.0f;

    protected float originalRectTransfmOffsetMaxDotY;
    protected float originalRectTransformOffsetMinDotY;

    [SerializeField]
    private Color BtnDownTextColor = Color.white;

    protected Color btnUpTextColor = Color.white;

    public EventTrigger eventTrigger;

    private void OnEnable()
    {
        rectTransform.localScale = normalSize;
    }

    protected void Awake()
    {
        // TODO: 'rectTransform.offsetMax.y;' or somehing should be better but didn't work at the first attempt
        originalRectTransfmOffsetMaxDotY = rectTransformToGoDownWhileBtnPressed.offsetMax.y;
        originalRectTransformOffsetMinDotY = rectTransformToGoDownWhileBtnPressed.offsetMin.y;

        if (textComponent != null)
        {
            btnUpTextColor = textComponent.color;
        }
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
        float yMax = originalRectTransfmOffsetMaxDotY - textVerticalDesloc;
        float yMin = originalRectTransformOffsetMinDotY - textVerticalDesloc;
        ConfigureBtnLooks(clickedSprite, yMax, yMin, BtnDownTextColor);
        soundRequisitor.RequestSFX(btnClickSound);
    }

    public virtual void OnPointerUp()
    {
        ConfigureBtnLooks(normalSprite, originalRectTransfmOffsetMaxDotY, originalRectTransformOffsetMinDotY, btnUpTextColor);
    }

    protected void ConfigureBtnLooks(Sprite sprite, float top, float bottom, Color color)
    {
        imageComponent.sprite = sprite;
        SetTop(top, bottom);
        if (textComponent != null)
        {
            textComponent.color = color;
        }
    }

    public void SetTop(float top, float bottom)
    {
        float x = rectTransformToGoDownWhileBtnPressed.offsetMax.x;
        rectTransformToGoDownWhileBtnPressed.offsetMax = new Vector2(x, top);
        x = rectTransformToGoDownWhileBtnPressed.offsetMin.x;
        rectTransformToGoDownWhileBtnPressed.offsetMin = new Vector2(x, bottom);
    }
}
