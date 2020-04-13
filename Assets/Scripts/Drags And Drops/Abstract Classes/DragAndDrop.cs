using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragAndDrop : MonoBehaviour
{
    #region Attributes
    [SerializeField]
    private RectTransform rectTransform = null;

    protected DragAndDropReceptor receptor;

    private bool snap;

    protected Vector3 originalPosition;

    // Offset for the image to not teleport to the mouse on click
    private Vector3 offset;
    #endregion

    protected virtual void Update()
    {
        if (snap)
        {
            rectTransform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerDown()
    {
        StartDragging();
    }

    public void StartDragging()
    {
        originalPosition = rectTransform.position;
        snap = true;
        offset = originalPosition - Input.mousePosition;
        OnStartDragging();
    }

    protected abstract void OnStartDragging();

    public void OnPointerUp()
    {
        if (snap)
        {
            BeforeDrop();

            snap = false;

            rectTransform.position = originalPosition;

            if (receptor != null)
            {
                receptor.OnDroppedInReceptor();
            }

            OnDroppedSpecificBehaviour();
        }
    }

    protected abstract void BeforeDrop();

    protected abstract void OnDroppedSpecificBehaviour();

    private void OnTriggerEnter2D(Collider2D col)
    {
        DragAndDropReceptor maybeAReceptor = col.GetComponent<DragAndDropReceptor>();

        if (maybeAReceptor != null && maybeAReceptor.GetDragAndDropReceptorType() == GetDragAndDropType())
        {
            this.receptor = maybeAReceptor;
            OnEnteredAReceptor(maybeAReceptor);
        }
    }

    protected abstract void OnEnteredAReceptor(DragAndDropReceptor receptor);

    protected abstract Type GetDragAndDropType();

    private void OnTriggerExit2D(Collider2D col)
    {
        // You can exit a tooltipHolder and is still above another. Therefore, this check is made.
        if (ExitedAllRespectiveReceptors())
        {
            receptor = null;
            OnExitedAllReceptors();
        }
    }

    private bool ExitedAllRespectiveReceptors()
    {
        // Get all colliders that overlap this object's collider.
        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        int colliderCount = GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders);

        // Verify if one of them has a receptor component to this drag and drop
        bool exitedAllRespectiveReceptors = true;

        for (int i = 0; i < colliderCount; i++)
        {
            DragAndDropReceptor maybeAReceptor = colliders[i].GetComponent<DragAndDropReceptor>();

            if (maybeAReceptor != null && maybeAReceptor.GetDragAndDropReceptorType() == GetDragAndDropType())
            {
                // It is a receptor for this 'DragAndDrop'
                exitedAllRespectiveReceptors = false;
                break;
            }
        }

        return exitedAllRespectiveReceptors;
    }

    protected abstract void OnExitedAllReceptors();
}