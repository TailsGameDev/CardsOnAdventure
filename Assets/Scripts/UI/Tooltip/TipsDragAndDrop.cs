using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class TipsDragAndDrop : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private Text text = null;

    private bool snap;

    private TipHolder tipHolder;

    private Vector3 originalPosition;
    private Vector3 offset;

    private void Awake()
    {
        originalPosition = rectTransform.position;
        StartCoroutine(MakeFontNormalAfterTriggerExitEvent());
    }

    public void OnPointerDown()
    {
        snap = true;
        text.fontStyle = FontStyle.Italic;
        offset = originalPosition - Input.mousePosition;
    }

    public void OnPointerUp()
    {
        snap = false;

        if (tipHolder != null)
        {
            tipHolder.OpenTipPopUp();
        }

        rectTransform.position = originalPosition;

        StartCoroutine(MakeFontNormalAfterTriggerExitEvent());
    }

    IEnumerator MakeFontNormalAfterTriggerExitEvent()
    {
        yield return null;
        text.fontStyle = FontStyle.Normal;
        yield return null;
        text.fontStyle = FontStyle.Normal;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TipHolder tipHolder = col.GetComponent<TipHolder>();

        if (tipHolder != null)
        {
            this.tipHolder = tipHolder;
            text.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    // You can exit a tooltipHolder and is still above another. Therefore, this check is made.
    private void OnTriggerExit2D(Collider2D col)
    {
        if ( ExitedAllTooltipHolders() )
        {
            tipHolder = null;
            text.fontStyle = FontStyle.Italic;
        }
    }

    private bool ExitedAllTooltipHolders()
    {
        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        int colliderCount = GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders);
        return colliderCount == 0;
    }

    private void Update()
    {
        if (snap)
        {
            rectTransform.position = Input.mousePosition + offset;
        }
    }
}
