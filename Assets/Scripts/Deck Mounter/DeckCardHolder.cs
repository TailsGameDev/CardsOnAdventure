using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCardHolder : DynamicSizeScrollableCardHolder
{
    #region Initialization
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        int amountOfSlots = DeckPrototypeFactory.DefaultDeckSize;
        InitializeSlotsAndRectSize(amountOfSlots);
        InitializeCards(amountOfSlots);
    }
#endregion

    private void InitializeCards(int amountOfSlots)
    {
        cards = new Card[amountOfSlots];
        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndex(DeckPrototypeFactory.GetCloneOfTheRandomCard(), i);
        }
    }

    public void DestroySelectedCard()
    {
        int selectedIndex = GetSelectedIndex();
        if (selectedIndex != -1)
        {
            Destroy(cards[selectedIndex].gameObject);
            cards[selectedIndex] = null;
        }
        else
        {
            L.ogError("Tried to destroy selected card, but selected index is -1", this);
        }
    }

    public bool SelectedIndexIsFree()
    {
        int selectedIndex = GetSelectedIndex();
        return selectedIndex != -1 && cards[selectedIndex] == null;
    }

    public void PutCardInSelectedIndex(Card card)
    {
        PutCardInIndex(card, GetSelectedIndex());
    }

    public bool SelectedIndexIsOcupied()
    {
        int selectedIndex = GetSelectedIndex();
        return selectedIndex != -1 && cards[selectedIndex] != null;
    }

    public Card[] GetCards()
    {
        return cards;
    }
}