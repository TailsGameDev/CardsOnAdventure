using System.Collections;
using UnityEngine.UI;

public class CardsCollectionDisplayer : DynamicSizeScrollableCardHolder
{
    // Inherits InitializeSlotsAndRectSize

    private Text[] textForEachCardAmount = null;

    #region Initialization
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        cards = CardsCollection.GetClonesOfCardsOnPlayerCollection();

        InitializeSlotsAndRectSize(amountOfSlots: cards.Length);
        PopulateCardAmountTexts();
        RefreshCardsStats();
        PopulateSlotsWithCards();
        DeactivateBlockedCardsAndResize();
    }

    private void PopulateCardAmountTexts()
    {
        textForEachCardAmount = new Text[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            Text textCardAmountOfThisSlot = slots[i].GetComponentInChildren<Text>();
            textForEachCardAmount[i] = textCardAmountOfThisSlot;
            UpdateCardColorAndAmountText(i);
        }
    }
    private void RefreshCardsStats()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].RefreshStatsForThePlayer();
        }
    }
    private void PopulateSlotsWithCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            // Background
            Card slotBackground = cards[i].GetClone();
            slotBackground.MakeColorGray();
            ChildMaker.AdoptTeleportAndScale(slots[i], slotBackground.GetRectTransform());
            ChildMaker.CopySizeDelta(slots[i], slotBackground.GetRectTransform());
            
            // The actual card
            ChildMaker.AdoptTeleportAndScale(slots[i], cards[i].GetRectTransform());
            ChildMaker.CopySizeDelta(slots[i], cards[i].GetRectTransform());

        }
    }
    private void DeactivateBlockedCardsAndResize()
    {
        int deactivatedSlotsAmount = 0;
        int[] amountOfEachCard = CardsCollection.GetCardsCollectionAmounts();
        int totalAmountOfCards = amountOfEachCard.Length;
        for (int c = 0; c < totalAmountOfCards; c++)
        {
            if (CardsCollection.IsCardLocked(c))
            {
                slots[c].gameObject.SetActive(false);
                deactivatedSlotsAmount++;
            }
        }
        SetHorizontalSizeOfRect(amountOfSlots: totalAmountOfCards - deactivatedSlotsAmount);
    }
    #endregion

    private void UpdateCardColorAndAmountText(int indexToUpdate)
    {
        int[] amountOfEachCard = CardsCollection.GetCardsCollectionAmounts();
        int cardAmount = amountOfEachCard[indexToUpdate];
        textForEachCardAmount[indexToUpdate].text = cardAmount.ToString();

        if (cardAmount > 0)
        {
            cards[indexToUpdate].MakeColorDefault();
        }
        else
        {
            cards[indexToUpdate].MakeColorGray();
        }
    }

    public void UpdateColorAndAmountTextOfCard(Card card)
    {
        int index = GetIndexOfCard(card);
        UpdateCardColorAndAmountText(index);
    }

    public int GetAmountOfCardNotCurrentlyInDeck(Card card)
    {
        int index = GetIndexOfCard(card);
        int[] amountOfEachCard = CardsCollection.GetCardsCollectionAmounts();
        int amount = amountOfEachCard[index];
        return amount;
    }
    private int GetIndexOfCard(Card card)
    {
        int c;
        for (c = 0; c < cards.Length; c++)
        {
            if (cards[c].IsAnotherInstanceOf(card))
            {
                break;
            }
        }
        return c;
    }

    public void RemoveAmountOfCardFromCollection(Card card, int amountToRemove)
    {
        CardsCollection.SumToCurrentAmount(card, -amountToRemove);
    }
}