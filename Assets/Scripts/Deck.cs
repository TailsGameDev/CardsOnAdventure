using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    List<Card> cards = null;

    public bool IsEmpty()
    {
        return !ContainCards();
    }

    public bool ContainCards()
    {
        return cards.Count > 0;
    }

    public Card DrawCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}
