using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory instance;

    [SerializeField]
    private Card[] cardPrototypes = null;

    private void Awake()
    {
        if ( instance == null )
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Card[] GetRandomCards(int amount)
    {
        Card[] cards = new Card[amount];

        for(int i = 0; i < cards.Length; i++)
        {
            int randomIndex = Random.Range(0, instance.cardPrototypes.Length);
            Card prototype = instance.cardPrototypes[randomIndex];
            cards[i] = Instantiate(prototype).GetComponent<Card>();
        }

        return cards;
    }
}
