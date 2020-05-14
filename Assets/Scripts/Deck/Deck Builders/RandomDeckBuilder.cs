using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    public RandomDeckBuilder(int size) : base(size)
    {
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = GetCloneOfTheRandomCard();
        }

        return deck;
    }
}
