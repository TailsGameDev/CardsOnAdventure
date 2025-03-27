using System;
using UnityEngine;

public abstract class DragAndDrop : MonoBehaviour
{
    #region Attributes
    [SerializeField]
    private RectTransform rectTransform = null;

    protected DragAndDropReceptor receptor;
    private DragAndDropReceptor maybeAReceptor;

    private bool snap;

    protected Vector3 originalPosition;

    // Offset for the image to not teleport to the mouse on click
    private Vector3 offset;

    private delegate void DoForEachValidOverlappingReceptor();

    #endregion

    protected abstract Type GetDragAndDropType();

    protected virtual void Update()
    {
        if (snap)
        {
            rectTransform.position = Input.mousePosition + offset;

            IfIsAboveASingleReceptorMakeItBecomeTheReceptor();

            if (!PlayerInput.GetMouseButton0())
            {
                OnPointerUp();
            }
        }
    }
    private void IfIsAboveASingleReceptorMakeItBecomeTheReceptor()
    {
        int count = 0;
        ForEachOverlappingValidReceptorDo(() => { count++; });
        if (count == 1)
        {
            receptor = GetValidOverlappingReceptor();
        }
    }

    #region Drag
    public void OnPointerDown()
    {
        StartDragging();
    }
    public void StartDragging()
    {
        originalPosition = rectTransform.position;
        snap = true;
        offset = AdjustOffset(offsetToAdjust: originalPosition - Input.mousePosition);

        OnStartDragging();
    }
    private Vector2 AdjustOffset(Vector2 offsetToAdjust)
    {
        const float DIV_CONST = 8.0f;

        Vector2 draggedSizeDelta = rectTransform.sizeDelta;

        if (offsetToAdjust.x > draggedSizeDelta.x / DIV_CONST)
        {
            offsetToAdjust.x = draggedSizeDelta.x / DIV_CONST;
        }
        else if (offsetToAdjust.x < -draggedSizeDelta.x / DIV_CONST)
        {
            offsetToAdjust.x = -draggedSizeDelta.x / DIV_CONST;
        }

        if (offsetToAdjust.y > draggedSizeDelta.y / DIV_CONST)
        {
            offsetToAdjust.y = draggedSizeDelta.y / DIV_CONST;
        }
        else if (offsetToAdjust.y < -draggedSizeDelta.y / DIV_CONST)
        {
            offsetToAdjust.y = -draggedSizeDelta.y / DIV_CONST;
        }

        return offsetToAdjust;
    }
    protected abstract void OnStartDragging();
    #endregion

    #region Drop
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
            }
            else
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
    #endregion

    #region OnTriggerEnter (try to get receptor)
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
        return receptor == null || (receptor != null && receptor.Priority <= maybeAReceptor.Priority);
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
    #endregion

    #region OnTriggerEXIT (should receptor become null, or something else?
    private void OnTriggerExit2D(Collider2D col)
    {
        // if it's not my receptor I'm exiting, there is nothing to do.
        if (col.GetComponent<DragAndDropReceptor>() == receptor)
        {
            if (OverlapsNoReceptor())
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
                    if (receptorCandidate == receptor)
                    {
                        L.ogWarning(this, "Damn, How can this be overlapping something it exited?");
                    }
                    receptor = receptorCandidate;
                }
                else
                {
                    // I exited my receptor and I'm not overlapping nothing interesting enough
                    receptor = null;
                }
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
    #endregion

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
            dragAndDrop.ForEachOverlappingValidReceptorDo(() => { validOverlappingReceptor = dragAndDrop.maybeAReceptor; });

            return validOverlappingReceptor;
        }
    }

    private void ForEachOverlappingValidReceptorDo(DoForEachValidOverlappingReceptor doForEachValidReceptor)
    {
        // Get all colliders that overlap this object's collider.
        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D contactFilter = new ContactFilter2D();
        int colliderCount = GetComponent<Collider2D>().Overlap(contactFilter.NoFilter(), colliders);

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