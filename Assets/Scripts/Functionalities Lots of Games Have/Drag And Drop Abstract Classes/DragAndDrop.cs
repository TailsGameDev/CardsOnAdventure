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
        // maybe the OnTriggerEnter did not work, so
        if (receptor == null)
        {
            receptor = GetValidOverlappingReceptor();
        }

        if (receptor != null)
        {
            RectTransform receptorRectTransform = receptor.GetComponent<RectTransform>();
            StartDragging(receptorRectTransform);
        }
        else
        {
            L.ogError("Receptor is null, and I don't know why. I hope trying again will just work.\n", this);
        }
    }

    private void SearchForReceptorNow()
    {

    }

    public void StartDragging(RectTransform receptorRectTransform)
    {
        originalPosition = rectTransform.position;
        snap = true;
        offset = AdjustOffset(receptorRectTransform, offsetToAdjust: originalPosition - Input.mousePosition);

        OnStartDragging();
    }

    private Vector2 AdjustOffset(RectTransform receptorRectTransform, Vector2 offsetToAdjust)
    {
        const float DIV_CONST = 8.0f;

        Vector2 receptorSizeDelta = this.receptor.GetComponent<RectTransform>().sizeDelta;

        if (offsetToAdjust.x > receptorSizeDelta.x / DIV_CONST)
        {
            offsetToAdjust.x = receptorSizeDelta.x / DIV_CONST;
        }
        else if (offsetToAdjust.x < -receptorSizeDelta.x / DIV_CONST)
        {
            offsetToAdjust.x = -receptorSizeDelta.x / DIV_CONST;
        }

        if (offsetToAdjust.y > receptorSizeDelta.y / DIV_CONST)
        {
            offsetToAdjust.y = receptorSizeDelta.y / DIV_CONST;
        }
        else if (offsetToAdjust.y < -receptorSizeDelta.y / DIV_CONST)
        {
            offsetToAdjust.y = -receptorSizeDelta.y / DIV_CONST;
        }

        return offsetToAdjust;
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
        if (maybeAReceptor != null)
        {
            if (ConditionToReplace(maybeAReceptor))
            {
                RegisterIfIsReceptorAndCallWhosInterested(possibleReceptorCollider: col);
            }
        }
    }

    protected virtual bool ConditionToReplace(DragAndDropReceptor maybeAReceptor)
    {
        return receptor == null || (receptor!=null && receptor.Priority <= maybeAReceptor.Priority);
    }

    private void RegisterIfIsReceptorAndCallWhosInterested(Collider2D possibleReceptorCollider)
    {
        DragAndDropReceptor maybeAReceptor = possibleReceptorCollider.GetComponent<DragAndDropReceptor>();

        if (maybeAReceptor.GetDragAndDropReceptorType() == GetDragAndDropType())
        {
            this.receptor = maybeAReceptor;
            OnEnteredAReceptor(maybeAReceptor);
        }
    }

    protected abstract void OnEnteredAReceptor(DragAndDropReceptor receptor);

    protected abstract Type GetDragAndDropType();

    private void OnTriggerExit2D(Collider2D col)
    {
        if ( OverlapsNoReceptor() )
        {
            receptor = null;
            OnExitedAllReceptors();
        } 
        else
        {
            // If this does not hold a receptor, or if the overlappingReceptor priority is enough, cache the overlapping receptor
            DragAndDropReceptor receptorCandidate = GetValidOverlappingReceptor();
            if (receptor == null)
            {
                receptor = receptorCandidate;
            }
            else if (receptorCandidate != null && receptor.Priority <= receptorCandidate.Priority)
            {
                receptor = receptorCandidate;
            }
        }
    }

    private bool OverlapsNoReceptor()
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