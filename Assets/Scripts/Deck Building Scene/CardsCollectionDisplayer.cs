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

        InitializeSlotsAndRectSize(amountOfSlots: cards.Length);
        PopulateAmountOfEachCard();
        PopulateCardAmountTexts();
        ApplyPlayerBonuses();
        PopulateSlotsWithCards();
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
        textForEachCardAmount = new Text[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            Text textCardAmountOfThisSlot = slots[i].GetComponentInChildren<Text>();
            textForEachCardAmount[i] = textCardAmountOfThisSlot;
            UpdateCardColorAndAmountText(i);
        }
    }
    private void ApplyPlayerBonuses()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].ApplyPlayerBonuses();
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
                ChildMaker.AdoptAndScaleAndSmoothlyMoveToParentThenDestroyChild
                    (slots[slot], card.GetRectTransform(), repositionAnimationDurationInSeconds);
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
        coloredClone.RefreshStats();
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