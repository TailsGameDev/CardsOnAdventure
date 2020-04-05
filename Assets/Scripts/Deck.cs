using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    List<Card> cards = null;

    [SerializeField]
    private bool enemysDeck = false;

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;

        cards = new List<Card>();

        if (enemysDeck)
        {
            cards.AddRange(DeckPrototypeFactory.GetPreparedCardsForTheEnemy());
        }
        else
        {
            cards.AddRange(DeckPrototypeFactory.GetPreparedCardsForThePlayer());
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.position = transform.position;
            cards[i].GetComponent<RectTransform>().SetParent(transform, true);
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
