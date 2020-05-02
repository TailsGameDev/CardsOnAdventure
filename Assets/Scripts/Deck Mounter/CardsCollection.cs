using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class CardsCollection : DynamicSizeScrollableCardHolder
{
    // Inherits InitializeSlotsAndRectSize

    private Text[] textForEachCardAmount = null;

    private int[] amountOfEachCard = null;
    private Card[] cardsOfCollection = null;

    #region Initialization
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

        PopulateCardAmountTexts();

        PopulateSlotsWithCards();
    }
    private void PopulateAmountOfEachCard()
    {
        amountOfEachCard = new int[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            amountOfEachCard[i] = 1;
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
    private void PopulateSlotsWithCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            ChildMaker.AdoptTeleportAndScale(slots[i], cards[i].GetComponent<RectTransform>());
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
                Destroy(card.gameObject);
                return;
            }
        }

        L.ogError("Couldn't find card of same type to give the card back.",this);
    }
    public Card GetCloneOfSelectedCardAndManageState()
    {
        int selectedIndex = GetSelectedIndex();

        if (selectedIndex == -1 || amountOfEachCard[selectedIndex] <= 0)
        {
            L.ogError("shouldn't provide clone. selectedIndex: "+selectedIndex+
                "; card amount: "+ amountOfEachCard[selectedIndex], this);
        }

        amountOfEachCard[selectedIndex]--;
        UpdateCardColorAndAmountText(selectedIndex);

        return cards[selectedIndex].GetClone();
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
}