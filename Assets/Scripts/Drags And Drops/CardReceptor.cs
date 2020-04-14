using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReceptor : DragAndDropReceptor
{
    [SerializeField]
    private int index;

    [SerializeField]
    private CardsHolder cardsHolder = null;

    [SerializeField]
    private UIBattle uiBattle = null;

    public int Index { get => index; }

    public override Type GetDragAndDropReceptorType()
    {
        return typeof(CardDragAndDrop);
    }

    public override void OnDroppedInReceptor()
    {
        uiBattle.OnAnyCardsHolderDrop(cardsHolder, index);
    }
}
