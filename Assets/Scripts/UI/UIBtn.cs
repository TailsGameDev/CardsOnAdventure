using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtn : MonoBehaviour
{
    [SerializeField]
    private Image imageComponent = null;

    private RectTransform rectTransform = null;

    [SerializeField]
    private Sprite clickedSprite = null;

    [SerializeField]
    private Sprite normalSprite = null;

    [SerializeField]
    private Vector3 bigSize;

    private Vector3 normalSize = new Vector3(1.0f, 1.0f, 1.0f);

    private void Awake()
    {
        rectTransform = imageComponent.GetComponent<RectTransform>();
    }

    public void OnPointerEntered()
    {
        rectTransform.localScale = bigSize;
    }

    public void OnPointerExited()
    {
        rectTransform.localScale = normalSize;
    }

    public void OnPointerClicked()
    {

    }
}
