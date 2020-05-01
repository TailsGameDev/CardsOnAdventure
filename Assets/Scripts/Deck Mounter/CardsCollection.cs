using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class CardsCollection : DynamicSizeScrollableCardHolder
{
    // Inherits InitializeSlotsAndRectSize

    /*
    [SerializeField]
    private Text prototypeOfCardsAmountText = null;
    */
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

    private IEnumerator PopulateSlotsWithCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            // It's not necessary to wait a frame. Just makes an effect of each card being placed, instead of all in once
            // yield return null;
            ChildMaker.AdoptTeleportAndScale(slots[i], cards[i].GetComponent<RectTransform>());
        }
        yield return null;
    }
    #endregion

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

    // Note this is not exactly the opposite of SelectedUnavailableCard
    public bool SelectedAvailableCard()
    {
        int selectedIndex = GetSelectedIndex();
        return selectedIndex != -1 && amountOfEachCard[selectedIndex] > 0;
    }

    // Note this is not exactly the opposite of SelectedAvailableCard
    public bool SelectedUnavailableCard()
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

    public Card GetCloneOfSelectedCard()
    {
        int selectedIndex = GetSelectedIndex();

        if (selectedIndex == -1 || amountOfEachCard[selectedIndex] <= 0)
        {
            L.ogError("shouldn't provide clone. selectedIndex: "+selectedIndex+
                "; card amount: "+ amountOfEachCard[selectedIndex], this);
        }

        Card selectedCard = cards[selectedIndex];
        Card clone = Instantiate(selectedCard);

        amountOfEachCard[selectedIndex]--;
        UpdateCardColorAndAmountText(selectedIndex);

        return clone;
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
}