using System;
using UnityEngine;

public class CardDragAndDrop : DragAndDrop
{
    [SerializeField]
    private Card card;

    private Transform originalParent;

    private bool isDragging;

    private bool forceReceptorToNullBeforeDrop = false;

    public bool IsDragging { get => isDragging; }
    public bool ForceReceptorToNullBeforeDrop { set => forceReceptorToNullBeforeDrop = value; }

    protected override void Update()
    {
        base.Update();
        
        if (isDragging)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        
    }

    protected override Type GetDragAndDropType()
    {
        return typeof(CardDragAndDrop);
    }

    protected override void OnStartDragging()
    {
        isDragging = true;
      
        originalParent = transform.parent;

        transform.SetParent(UIBattle.parentOfDynamicUIThatMustAppear);
    }

    protected override void BeforeDrop()
    {
        if (forceReceptorToNullBeforeDrop)
        {
            receptor = null;
        }
        transform.SetParent(originalParent);
        transform.localScale = Vector3.one;
    }

    protected override void OnDroppedSpecificBehaviour()
    {
        isDragging = false;
    }

    protected override void OnEnteredAReceptor(DragAndDropReceptor receptor)
    {
    }

    protected override void OnExitedAllReceptors()
    {
    }
}
