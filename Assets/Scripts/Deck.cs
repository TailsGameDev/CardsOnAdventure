using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private bool makeRandomDeckInStart = false;

    [SerializeField]
    List<Card> cards = null;

    private void Start()
    {
        if (makeRandomDeckInStart)
        {
            int size = cards.Count;
            cards = new List<Card>();
            cards.AddRange(DeckPrototypeFactory.GetRandomCards(size));
            for (int i= 0; i < cards.Count; i++)
            {
                cards[i].transform.position = transform.position;
                cards[i].GetComponent<RectTransform>().SetParent(transform, true);
            }
        }
    }

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
