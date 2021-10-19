using System.Collections;
using UnityEngine.UI;

public class CardsCollectionDisplayer : DynamicSizeScrollableCardHolder
{
    // Inherits InitializeSlotsAndRectSize

    private Text[] textForEachCardAmount = null;

    private int[] amountOfEachCard = null;

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

        InitializeSlotsRectSizeAndTransformWrapper(amountOfSlots: cards.Length);
        PopulateAmountOfEachCard();
        PopulateCardAmountTexts();
        RefreshCardsStats();
        PopulateSlotsWithCards();
        DeactivateBlockedCardsAndResize();
    }
    private void PopulateAmountOfEachCard()
    {
        amountOfEachCard = new int[cards.Length];

        int[] cardAmounts = CardsCollection.GetCardsCollectionAmounts();

        Card[] currentDeck = PlayerAndEnemyDeckHolder.GetPreparedCardsForThePlayerWithTheRandomCards();

        // For each card on Deck, subtract 1 from it's amount in the collection.
        for (int i = 0; i < cards.Length; i++)
        {
            int amount = cardAmounts[i];
            for (int k = 0; k < currentDeck.Length; k++)
            {
                if (cards[i].IsAnotherInstanceOf(currentDeck[k]))
                {
                    amount--;
                }
            }

            amountOfEachCard[i] = amount;
        }
    }
    private void PopulateCardAmountTexts()
    {
        textForEachCardAmount = new Text[slotWrappers.Length];
        for (int i = 0; i < slotWrappers.Length; i++)
        {
            Text textCardAmountOfThisSlot = slotWrappers[i].GetTextInChildren();
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
            ChildMaker.AdoptTeleportAndScale(slotWrappers[i], slotBackground.TransformWrapper);
            ChildMaker.CopySizeDelta(slotWrappers[i].GetRectTransform(), slotBackground.GetRectTransform());
            
            // The actual card
            ChildMaker.AdoptTeleportAndScale(slotWrappers[i], cards[i].TransformWrapper);
            ChildMaker.CopySizeDelta(slotWrappers[i].GetRectTransform(), cards[i].GetRectTransform());

        }
    }
    private void DeactivateBlockedCardsAndResize()
    {
        int deactivatedSlotsAmount = 0;
        int totalAmountOfCards = amountOfEachCard.Length;
        for (int c = 0; c < totalAmountOfCards; c++)
        {
            if (CardsCollection.IsCardLocked(c))
            {
                slotWrappers[c].DeactivateGameObject();
                deactivatedSlotsAmount++;
            }
        }
        SetHorizontalSizeOfRect(amountOfSlots: totalAmountOfCards - deactivatedSlotsAmount);
    }
    #endregion

    // Note this is not exactly the opposite of SelectedUnavailableCard
    public bool AvailableCardWasSelected()
    {
        int selectedIndex = GetSelectedIndex();
        return selectedIndex != -1 && amountOfEachCard[selectedIndex] > 0;
    }
    // Note this is not exactly the opposite of SelectedAvailableCard
    public bool UnavailableCardWasSelected()
    {
        int selectedIndex = GetSelectedIndex();
        if (selectedIndex == -1)
        {
            // Not selected any card at all, so did not selected an unavailable card.
            return false;
        }
        else
        {
            if (amountOfEachCard != null)
            {
                return amountOfEachCard[selectedIndex] <= 0;
            }
            else
            {
                // He selected a card before the array was defined. This is rare.
                // In this case, let's say he didn't selected an unavailable card,
                // And hope the precise information is returned in the next frame.
                L.ogError("Selected card before initialization was completed", this);
                return false;
            }
        }
    }
    public void GiveCardBack(Card card)
    {
        for (int slot = 0; slot < cards.Length; slot++)
        {
            Card cardInSlot = GetReferenceToCardAt(slot);
            if (card.IsAnotherInstanceOf(cardInSlot))
            {
                amountOfEachCard[slot] ++ ;
                UpdateCardColorAndAmountText(slot);
                ChildMaker.AdoptAndScaleAndSmoothlyMoveToParent
                    (slotWrappers[slot], card.TransformWrapper, repositionAnimationDurationInSeconds);
                const float DELAY_FROM_ANIMATION_END_TO_DESTRUCTION = 0.2f;
                ObjectDestroyer.DestroyObjectInTime(card.gameObject, repositionAnimationDurationInSeconds + DELAY_FROM_ANIMATION_END_TO_DESTRUCTION);
                return;
            }
        }

        L.ogError("Couldn't find card of same type to give the card back.",this);
    }

    public Card GetCloneOfSelectedCardAndReduceAmountInDeck()
    {
        int selectedIndex = GetSelectedIndex();

        if (selectedIndex == -1 || amountOfEachCard[selectedIndex] <= 0)
        {
            L.ogError("shouldn't provide clone. selectedIndex: "+selectedIndex+
                "; card amount: "+ amountOfEachCard[selectedIndex], this);
        }

        amountOfEachCard[selectedIndex]--;

        Card coloredClone = cards[selectedIndex].GetClone();
        coloredClone.RefreshStatsForThePlayer();
        UpdateCardColorAndAmountText(selectedIndex);

        return coloredClone;
    }
    private void UpdateCardColorAndAmountText(int indexToUpdate)
    {
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
        int index = GetIndexOfCard(card);
        amountOfEachCard[index] -= amountToRemove;
        CardsCollection.SumToCurrentAmount(card, -amountToRemove);
    }
}