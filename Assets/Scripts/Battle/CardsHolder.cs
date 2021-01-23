using UnityEngine;

public class CardsHolder : IndexHolder
{
    [SerializeField]
    protected RectTransform[] cardPositions = null;

    protected Card[] cards = new Card[4];

    [SerializeField]
    protected UICardsHolderEventHandler uiCardsHolderEventHandler = null;

    [SerializeField]
    protected float repositionAnimationDurationInSeconds = 0.25f;

    #region Collection Default Opperations
    public int GetSize()
    {
        return cards.Length;
    }

    public void PutCardInIndexWithSmoothMovement(Card card, int index)
    {
        PutCardInIndex(card, index, smooth: true);
    }

    public void PutCardInIndexThenTeleportToSlot(Card card, int index)
    {
        PutCardInIndex(card, index, smooth: false);
    }

    private void PutCardInIndex(Card card, int index, bool smooth)
    {
        RectTransform cardRect = card.GetRectTransform();

        cards[index] = card;
        cardRect.rotation = transform.rotation;

        if (smooth)
        {
            ChildMaker.AdoptAndScaleAndSmoothlyMoveToParent(cardPositions[index].transform, card.GetRectTransform(), repositionAnimationDurationInSeconds);
        }
        else
        {
            ChildMaker.AdoptAndTeleport(cardPositions[index].transform, card.GetRectTransform());
        }

        cardRect.localScale = new Vector3(1, 1, 1);
        Rect slotRect = cardPositions[index].rect;
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
    public int GetAmountOfCardsThatCanAttack()
    {
        int canAttack = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null && !cards[i].Freezing)
            {
                canAttack++;
            }
        }
        return canAttack;
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
    public void PutCardInSelectedIndexWithSmoothMovement(Card card)
    {
        PutCardInIndex(card, GetSelectedIndex(), smooth: true);
    }
    public void PutCardInSelectedIndexThenTeleportToSlot(Card card)
    {
        PutCardInIndex(card, GetSelectedIndex(), smooth: false);
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
            cards[index].RectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
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
            cards[index].RectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
    #endregion

    public void RemoveFreezingStateFromAllCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (ContainsCardInIndex(i))
            {
                cards[i].RemoveFreezing();
            }
        }
    }
    public void RemoveObfuscateFromAllCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                cards[i].SetObfuscate(false);
            }
        }
    }
}
