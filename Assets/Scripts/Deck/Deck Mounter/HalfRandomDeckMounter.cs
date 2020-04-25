using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfRandomDeckMounter : DeckPrototypeFactory.DeckMounter
{
    private Classes classe;

    public HalfRandomDeckMounter(int size, Classes classe) : base(size)
    {
        this.classe = classe;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        Card[] notRandomPartPrototypes = ClassInfo.GetCardsOfClass(classe);

        if (notRandomPartPrototypes == null || notRandomPartPrototypes.Length == 0)
        {
            Debug.LogError("[DeckPrototypeFactory] notRandomPartPrototypes.Length == 0. " +
                            "It should be the size of the available cards of a class.");
        }

        return BuildHalfRandomDeck(notRandomPartPrototypes);
    }

    private Card[] BuildHalfRandomDeck(Card[] notRandomPartPrototypes)
    {
        deck = InOrderBuildRangeWithPrototypes(beginningIndex: 0, limitIndex: size / 2, notRandomPartPrototypes);

        Card[] allPrototypesThereAre = allCardPrototypes;
        deck = OutOfOrderBuildRangeWithPrototypes(beginningIndex: size / 2, limitIndex: size, allPrototypesThereAre);

        Shuffle(ref deck);

        return deck;
    }
}
