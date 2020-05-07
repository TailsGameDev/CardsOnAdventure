using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : CardsHolder
{
    public void AddCard(Card card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                PutCardInIndex(card, i, smooth: true);
                break;
            }
        }
    }

    public void SelectFirstOccupiedIndex()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                SetSelectedIndex(i);
                break;
            }
        }
    }
}
