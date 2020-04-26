using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOfEachCardDeckMounter : DeckPrototypeFactory.DeckBuilder
{
    private OneOfEachCardDeckMounter(int size) : base(size)
    {
    }

    public static OneOfEachCardDeckMounter Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachCardDeckMounter me = new OneOfEachCardDeckMounter(SENTINEL_NUMBER);
        me.size = me.allCardPrototypes.Length;
        return me;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = InstantiatePlease(allCardPrototypes[i]);
        }

        return deck;
    }
}