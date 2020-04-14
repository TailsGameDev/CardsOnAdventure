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

    private delegate void DoForEachValidOverlappingReceptor();

    private DragAndDropReceptor maybeAReceptor;
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
        Drop();
    }

    public void Drop()
    {
        if (snap)
        {
            BeforeDrop();

            snap = false;

            if (this.receptor != null)
            {
                this.receptor.OnDroppedInReceptor();
            } else
            {
                ReturnToOriginalPosition();
            }

            OnDroppedSpecificBehaviour();

            this.receptor = null;
        }
    }

    protected abstract void BeforeDrop();

    public void ReturnToOriginalPosition()
    {
        rectTransform.position = originalPosition;
    }

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
        if ( ExitedAllValidReceptors() )
        {
            receptor = null;
            OnExitedAllReceptors();
        } 
        else
        {
            receptor = GetValidOverlappingReceptor();
        }
    }

    private bool ExitedAllValidReceptors()
    {
        return new ExitedAllValidReceptorsVerifier(this).Verify();
    }

    protected abstract void OnExitedAllReceptors();

    private DragAndDropReceptor GetValidOverlappingReceptor()
    {
        return new ValidOverlappingReceptorGetter(this).Get();
    }

    private class ExitedAllValidReceptorsVerifier
    {
        private DragAndDrop dragAndDrop;

        private bool exitedAllRespectiveReceptors;

        public ExitedAllValidReceptorsVerifier(DragAndDrop dragAndDrop)
        {
            this.dragAndDrop = dragAndDrop;
        }

        public bool Verify()
        {
            exitedAllRespectiveReceptors = true;

            dragAndDrop.ForEachOverlappingValidReceptorDo(() => { exitedAllRespectiveReceptors = false; });

            return exitedAllRespectiveReceptors;
        }
    }

    private class ValidOverlappingReceptorGetter
    {
        private DragAndDrop dragAndDrop;

        private DragAndDropReceptor validOverlappingReceptor;

        public ValidOverlappingReceptorGetter(DragAndDrop dragAndDrop)
        {
            this.dragAndDrop = dragAndDrop;
        }

        public DragAndDropReceptor Get()
        {
            dragAndDrop.ForEachOverlappingValidReceptorDo( () => { validOverlappingReceptor = dragAndDrop.maybeAReceptor; } );

            return validOverlappingReceptor;
        }
    }

    private void ForEachOverlappingValidReceptorDo(DoForEachValidOverlappingReceptor doForEachValidReceptor)
    {
        // Get all colliders that overlap this object's collider.
        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        int colliderCount = GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            maybeAReceptor = colliders[i].GetComponent<DragAndDropReceptor>();

            if (maybeAReceptor != null && maybeAReceptor.GetDragAndDropReceptorType() == GetDragAndDropType())
            {
                doForEachValidReceptor();
            }
        }
    }
}