using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsHolder : IndexHolder
{
    [SerializeField]
    protected Transform[] cardPositions = null;

    protected Card[] cards = new Card[4];

    [SerializeField]
    protected UICardsHolderEventHandler uiBattle = null;

    // to activate animation, uncomment this field and toggle the ChildMaker algorythm inside 'PutCardInIndex' method
    // [SerializeField]
    // private float repositionAnimationDurationInSeconds = 0.5f;

    #region Collection Default Opperations
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

    public Card GetReferenceToCardAt(int index)
    {
        if (cards[index] == null)
        {
            Debug.LogError("[Battlefield] trying to get reference to a null card.", this);
        }

        return cards[index];
    }

    public Card RemoveCardFromSelectedIndex()
    {
        return RemoveCardOrGetNull(GetSelectedIndex());
    }

    public Card RemoveCardOrGetNull(int index)
    {
        Card card = cards[index];
        cards[index] = null;
        return card;
    }

    public bool ContainsCardInIndex(int index)
    {
        return cards[index] != null;
    }

    public int GetStandingAmount()
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
    #endregion

    public void SelectFirstOccupiedIndex()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                SetSelectedIndex(i);
                break;
            }
        }
    }

    public void OnSlotBeginDrag(int index)
    {
        uiBattle.OnCardsHolderBeginDrag(this, index);
    }

    public void OnSlotEndDrag()
    {
        uiBattle.OnCardsHolderEndDrag();
    }

    public void OnDroppedInSlot(int index)
    {
        uiBattle.OnCardsHolderDrop(this, index);
    }

    public void OnSlotClicked(int index)
    {
        uiBattle.OnSlotClicked(this, index);
    }

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

    internal void MakeSelectedCardNormalSize()
    {
        MakeCardAtIndexNormalSize(GetSelectedIndex());
    }

    internal void MakeCardAtIndexNormalSize(int index)
    {
        if (index != -1 && cards[index] != null)
        {
            cards[index].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
    #endregion
}
