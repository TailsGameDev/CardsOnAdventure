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

    private Card cardAboveReceptor;

    public Card CardAboveReceptor { set => cardAboveReceptor = value; }

    public override Type GetDragAndDropReceptorType()
    {
        return typeof(CardDragAndDrop);
    }

    public override void OnDroppedInReceptor()
    {
        if (cardAboveReceptor != null)
        {
            // cardsHolder.PutCardInIndex(cardAboveReceptor, index);
            cardsHolder.SetSelectedIndex(index);
        }
        else
        {
            Debug.LogError("[CardReceptor] cardAboveReceptor shouldn't be null!!", this);
        }
    }
}
