using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsHolder : IndexHolder
{
    [SerializeField]
    protected Transform[] cardPositions = null;

    protected Card[] cards = new Card[4];

    [SerializeField]
    private float repositionAnimationDurationInSeconds = 0.5f;

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

    public bool ContainsCardInIndex(int index)
    {
        return cards[index] != null;
    }

    public void PutCardInIndex(Card card, int index)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();

        cards[index] = card;
        cardRect.rotation = transform.rotation;

        // ChildMaker.AdoptAndSmoothlyMoveToParent(cardPositions[index].transform, card.GetComponent<RectTransform>(), repositionAnimationDurationInSeconds);
        ChildMaker.AdoptAndTeleport(cardPositions[index].transform, card.GetComponent<RectTransform>());

        cardRect.localScale = new Vector3(1,1,1);
        Rect slotRect = cardPositions[index].GetComponent<RectTransform>().rect;
        cardRect.sizeDelta = new Vector2(slotRect.width, slotRect.height);
    }

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

    #region Make card bigger or smaller
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
