using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : CardsHolder
{

    public bool IsSlotIndexFree(int slotIndex)
    {
        return cards[slotIndex] == null;
    }

    public void PlaceCardInSelectedIndex(Card card)
    {
        PutCardInIndex(card, GetSelectedIndex());
    }

    public void SelectFirstFreeIndex()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                SetSelectedIndex(i);
                break;
            }
        }
    }

    public Card GetReferenceToCardAt(int index)
    {
        if (cards[index] == null)
        {
            Debug.LogError("[Battlefield] trying to get reference to a null card.", this);
        }

        return cards[index];
    }
}
