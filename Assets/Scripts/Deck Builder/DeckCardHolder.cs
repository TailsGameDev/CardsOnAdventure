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
    private void InitializeCards(int amountOfSlots)
    {
        cards = new Card[amountOfSlots];
        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndex(DeckPrototypeFactory.GetCloneOfTheRandomCard(), i, smooth: true);
        }
    }
    #endregion

    public Card[] GetCards()
    {
        return cards;
    }
}