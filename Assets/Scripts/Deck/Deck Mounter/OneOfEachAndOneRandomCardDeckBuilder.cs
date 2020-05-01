using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOfEachAndOneRandomCardDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private OneOfEachAndOneRandomCardDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachAndOneRandomCardDeckBuilder Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachAndOneRandomCardDeckBuilder me = new OneOfEachAndOneRandomCardDeckBuilder(SENTINEL_NUMBER);
        me.size = me.allCardPrototypes.Length+1;
        return me;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length-1; i++)
        {
            deck[i] = allCardPrototypes[i].GetClone();
        }
        deck[size - 1] = GetCloneOfTheRandomCard();

        return deck;
    }
}
