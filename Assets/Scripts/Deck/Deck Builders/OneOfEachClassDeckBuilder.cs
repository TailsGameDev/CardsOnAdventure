using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOfEachClassDeckBuilder : DeckBuilderSuperclass
{
    private Card[] prototypes;
    private OneOfEachClassDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachClassDeckBuilder Create(Classes classe)
    {
        Card[] prototypes = ClassInfo.GetCardsOfClass(classe);
        OneOfEachClassDeckBuilder builder = new OneOfEachClassDeckBuilder(prototypes.Length);
        builder.prototypes = prototypes;
        return builder;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < prototypes.Length; i++)
        {
            deck[i] = prototypes[i].GetClone();
        }

        return deck;
    }
}
