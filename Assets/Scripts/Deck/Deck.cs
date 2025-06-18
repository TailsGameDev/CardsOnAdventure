using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private List<Card> cards = null;

    [SerializeField]
    private bool enemysDeck = false;

    private int initialHP;

    private IEnumerator Start()
    {
        yield return null;

        // Populate deck and set initial HP
        cards = new List<Card>();

        if (enemysDeck)
        {
            cards.AddRange(PlayerAndEnemyDeckHolder.GetPreparedCardsForTheEnemy());
            initialHP = PlayerAndEnemyDeckHolder.GetEnemyInitialHP();
        }
        else
        {
            cards.AddRange(PlayerAndEnemyDeckHolder.GetPreparedCardsForThePlayerOrGetRandomDeck());
            initialHP = PlayerAndEnemyDeckHolder.GetPlayerInitialHP();
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].RectTransform.position = transform.position;
            cards[i].RectTransform.SetParent(transform, true);
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

    public int GetSize()
    {
        return cards.Count;
    }

    public Card DrawCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void PutCardInTop(Card card)
    {
        cards.Insert(0, card);
    }

    public int GetInitialHP()
    {
        return initialHP;
    }
}
