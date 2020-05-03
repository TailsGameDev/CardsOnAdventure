using UnityEngine;

public class CardsHolder : IndexHolder
{
    [SerializeField]
    protected Transform[] cardPositions = null;

    protected Card[] cards = new Card[4];

    [SerializeField]
    protected UICardsHolderEventHandler uiCardsHolderEventHandler = null;

    // to activate animation, uncomment this field and toggle the ChildMaker algorythm inside 'PutCardInIndex' method
    // [SerializeField]
    // private float repositionAnimationDurationInSeconds = 0.5f;

    #region Collection Default Opperations
    public int GetSize()
    {
        return cards.Length;
    }

    public void PutCardInIndex(Card card, int index)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();

        cards[index] = card;
        cardRect.rotation = transform.rotation;

        // ChildMaker.AdoptAndSmoothlyMoveToParent(cardPositions[index].transform, card.GetComponent<RectTransform>(), repositionAnimationDurationInSeconds);
        ChildMaker.AdoptAndTeleport(cardPositions[index].transform, card.GetComponent<RectTransform>());

        cardRect.localScale = new Vector3(1, 1, 1);
        Rect slotRect = cardPositions[index].GetComponent<RectTransform>().rect;
        cardRect.sizeDelta = new Vector2(slotRect.width, slotRect.height);
    }

    public void Remove(object card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
#pragma warning disable CS0253 // Possível comparação de referência inesperada; o lado direito precisa de conversão
            if (cards[i] == card)
#pragma warning restore CS0253 // Possível comparação de referência inesperada; o lado direito precisa de conversão
            {
                cards[i] = null;
                return;
            }
        }
        L.ogError(this,"No card was removed at Remove method!!");
    }
    public Card RemoveCardOrGetNull(int index)
    {
        Card card = cards[index];
        cards[index] = null;
        return card;
    }

    public Card GetReferenceToCardAt(int index)
    {
        if (cards[index] == null)
        {
            Debug.LogError("[Battlefield] trying to get reference to a null card.", this);
        }

        return cards[index];
    }
    public Card GetReferenceToCardAtOrGetNull(int index)
    {
        return cards[index];
    }

    public bool ContainsCardInIndex(int index)
    {
        return cards[index] != null;
    }
    public bool IsSlotIndexFree(int slotIndex)
    {
        return cards[slotIndex] == null;
    }
    public bool IsSlotIndexOccupied(int slotIndex)
    {
        return cards[slotIndex] != null;
    }
    public int GetAmountOfOccupiedSlots()
    {
        int standing = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                standing++;
            }
        }
        return standing;
    }
    public int GetFirstOccupiedIndex()
    {
        int occupied = -1;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                occupied = i;
                break;
            }
        }
        return occupied;
    }

    public bool IsFull()
    {
        bool full = true;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                full = false;
            }
        }
        return full;
    }
    public bool IsEmpty()
    {
        bool empty = true;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                empty = false;
                break;
            }
        }
        return empty;
    }
    public bool HasEmptySlot()
    {
        return !IsFull();
    }
    public bool HasCards()
    {
        return !IsEmpty();
    }
    #endregion

    #region Operations that Consider Selection And Don't deal with Size
    public void PutCardInSelectedIndex(Card card)
    {
        PutCardInIndex(card, GetSelectedIndex());
    }
    public Card RemoveCardFromSelectedIndex()
    {
        return RemoveCardOrGetNull(GetSelectedIndex());
    }
    public bool SomeIndexWasSelected()
    {
        return GetSelectedIndex() != -1;
    }
    public bool SelectionIsCleared()
    {
        return GetSelectedIndex() == -1;
    }
    public Card GetSelectedCard()
    {
        return cards[GetSelectedIndex()];
    }
    public void SelectFirstFreeIndex()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                SetSelectedIndex(i);
                break;
            }
        }
    }
    #endregion

    #region Slot Events
    public void OnSlotBeginDrag(int index)
    {
        uiCardsHolderEventHandler.OnCardsHolderBeginDrag(this, index);
    }
    public void OnSlotEndDrag()
    {
        uiCardsHolderEventHandler.OnCardsHolderEndDrag();
    }
    public void OnDroppedInSlot(int index)
    {
        uiCardsHolderEventHandler.OnCardsHolderDrop(this, index);
    }
    public void OnSlotClicked(int index)
    {
        uiCardsHolderEventHandler.OnSlotClicked(this, index);
    }
    #endregion

    #region Make card bigger or normal size
    public void MakeOnlySelectedCardBigger()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            MakeCardAtIndexNormalSize(i);
        }
        MakeSelectedCardBigger();
    }
    public void MakeSelectedCardBigger()
    {
        MakeCardAtIndexBigger(GetSelectedIndex());
    }
    public void MakeCardAtIndexBigger(int index)
    {
        if (index != -1 && cards[index] != null)
        {
            cards[index].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }
    public void MakeSelectedCardNormalSize()
    {
        MakeCardAtIndexNormalSize(GetSelectedIndex());
    }
    public void MakeCardAtIndexNormalSize(int index)
    {
        if (index != -1 && cards[index] != null)
        {
            cards[index].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
    #endregion
}
