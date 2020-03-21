using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsHolder : IndexHolder
{
    [SerializeField]
    protected Transform[] cardPositions = null;

    protected Card[] cards = new Card[4];

    public Card RemoveCardFromSelectedIndex()
    {
        return RemoveCard(GetSelectedIndex());
    }

    public Card RemoveCard(int index)
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
        cards[index] = card;
        card.transform.position = cardPositions[index].position;
        card.GetComponent<RectTransform>().SetParent(cardPositions[index], true);
    }
}
