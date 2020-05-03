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

        Card[] prototypes = allCardPrototypes;

        deck = BuildFullRandomDeckFromPrototypes(allCardPrototypes);

        return deck;
    }

    private Card[] BuildFullRandomDeckFromPrototypes(Card[] prototypes)
    {
        deck = OutOfOrderBuildRangeWithPrototypes(beginningIndex: 0, limitIndex: size, prototypes);

        return deck;
    }
}
