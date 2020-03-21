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

}
