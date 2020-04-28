using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class CardsCollection : DynamicSizeScrollableCardHolder
{
    // Inherits InitializeSlotsAndRectSize

    private int[] amountOfEachCard = null;
    private Card[] cardsOfCollection = null;

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        cards = DeckPrototypeFactory.GetCopyOfAllAndEachCardPrototypePlusTheRandomCard();

        cardsOfCollection = new Card[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            cardsOfCollection[i] = cards[i];
        }

        PopulateAmountOfEachCard();

        InitializeSlotsAndRectSize(amountOfSlots: cards.Length);

        yield return PopulateSlotsWithCards();
    }

    private void PopulateAmountOfEachCard()
    {
        amountOfEachCard = new int[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            amountOfEachCard[i] = 1;
        }
    }

    private IEnumerator PopulateSlotsWithCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            // It's not necessary to wait a frame. Just makes an effect of each card being placed, instead of all in once
            yield return null;
            ChildMaker.AdoptTeleportAndScale(slots[i], cards[i].GetComponent<RectTransform>());
        }
    }

    public bool SelectedIndexIsOcupied()
    {
        int selectedIndex = GetSelectedIndex();
        return selectedIndex != -1 && amountOfEachCard[selectedIndex] > 0;
    }

    public int GetIndexOfACardEqualTo(Card card)
    {
        int index = -1;
        for (int i = 0; i < cardsOfCollection.Length; i++)
        {
            if (card.IsAnotherInstanceOf(cardsOfCollection[i]))
            {
                index = i;
            }
        }
        if (index == -1)
        {
            L.ogError("Tryed to get index of card but there is not a card like this in the collection", this);
        }
        return index;
    }

    /*
    public Card GetSelectedCardOrGetNull()
    {
        int selectedIndex = GetSelectedIndex();
        if (selectedIndex != -1)
        {
            return cards[GetSelectedIndex()];
        }
        return null;
    }
    */
}