using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOfEachCardDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private OneOfEachCardDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachCardDeckBuilder Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachCardDeckBuilder me = new OneOfEachCardDeckBuilder(SENTINEL_NUMBER);
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