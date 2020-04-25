using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOfEachCardDeckMounter : DeckPrototypeFactory.DeckMounter
{
    private OneOfEachCardDeckMounter(int size) : base(size)
    {
    }

    public static OneOfEachCardDeckMounter New()
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