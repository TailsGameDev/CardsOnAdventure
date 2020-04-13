using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragAndDrop : DragAndDrop
{
    [SerializeField]
    private Card card;

    private Transform originalParent;

    protected override void Update()
    {
        base.Update();
        
        if (snap)
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
        originalParent = transform.parent;

        transform.parent = UIBattle.parentOfDynamicUIThatMustAppear;
    }

    protected override void BeforeDrop()
    {
        transform.parent = originalParent;
        transform.localScale = Vector3.one;
    }

    protected override void OnDroppedSpecificBehaviour()
    {
    }

    protected override void OnEnteredAReceptor(DragAndDropReceptor receptor)
    {
        ((CardReceptor)receptor).CardAboveReceptor = card;
    }

    protected override void OnExitedAllReceptors()
    {
    }
}
