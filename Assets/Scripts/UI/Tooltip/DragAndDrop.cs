using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;

    private bool snap;

    private TipHolder tipHolder;

    public void OnPointerDown ()
    {
        snap = true;
    }

    public void OnPointerUp()
    {
        snap = false;
        if (tipHolder != null)
        {
            tipHolder.OpenTipPopUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TipHolder tipHolder = col.GetComponent<TipHolder>();
        if (tipHolder != null)
        {
            this.tipHolder = tipHolder;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        int colliderCount = GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders);
        if (colliderCount == 0)
        {
            tipHolder = null;
            Debug.LogError("tipHolder is null");
        }
    }

    private void Update()
    {
        if (snap)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
}
