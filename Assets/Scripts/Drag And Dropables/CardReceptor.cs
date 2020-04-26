using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReceptor : DragAndDropReceptor
{
    [SerializeField]
    private int index = -1;

    [SerializeField]
    private CardsHolder cardsHolder = null;

    public int Index { get => index; set => index = value; }

    public CardsHolder CardsHolder { set => cardsHolder = value; }

    #region Overrides
    public override Type GetDragAndDropReceptorType()
    {
        return typeof(CardDragAndDrop);
    }

    public override void OnDroppedInReceptor()
    {
        cardsHolder.OnDroppedInSlot(index);
    }
    #endregion

    #region Events
    public void OnSlotBeginDrag()
    {
        cardsHolder.OnSlotBeginDrag(index);
    }

    public void OnSlotEndDrag()
    {
        cardsHolder.OnSlotEndDrag();
    }

    public void OnSlotClicked()
    {
        cardsHolder.OnSlotClicked(index);
    }
    #endregion
}
