using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollHelper : MonoBehaviour
{
    [SerializeField]
    Scrollbar scrollbar = null;

    [SerializeField]
    private float updateScrollTax = 0.0f;

    private void OnTriggerStay2D(Collider2D col)
    {
        CardDragAndDrop cardDragNDrop = col.GetComponent<CardDragAndDrop>();
        if (cardDragNDrop!=null && cardDragNDrop.IsDragging)
        {
            bool toLeft = updateScrollTax > 0 && scrollbar.value < 0.99f;
            bool toRight = updateScrollTax < 0 && scrollbar.value > 0.01f;

            if (toLeft || toRight)
            {
                scrollbar.value += updateScrollTax;
            }
        }
    }
}
