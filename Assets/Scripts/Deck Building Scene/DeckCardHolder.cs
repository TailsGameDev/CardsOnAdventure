using UnityEngine;
using System.Collections;

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
        int amountOfSlots = DecideAmountOfSlotsThisCardHolderShouldHave();
        InitializeSlotsAndRectSize(amountOfSlots);
        InitializeCards(slots);
    }
    protected virtual int DecideAmountOfSlotsThisCardHolderShouldHave()
    {
        int amountOfSlots = DeckPrototypeFactory.DefaultDeckSize;
        return amountOfSlots;
    }
    private void InitializeCards(RectTransform[] slots)
    {
        cards = PopulateCardsArray();

        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndexWithSmoothMovement(cards[i], i);

            ChildMaker.CopySizeDelta(slots[i], cards[i].GetRectTransform());
        }
    }
    protected virtual Card[] PopulateCardsArray()
    {
        cards = DeckPrototypeFactory.GetPreparedCardsForThePlayerWithTheRandomCards();
        return cards;
    }
    #endregion

    public Card[] GetCards()
    {
        return cards;
    }
}