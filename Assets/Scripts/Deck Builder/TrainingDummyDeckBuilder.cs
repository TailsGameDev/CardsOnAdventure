using UnityEngine;
using System.Collections;

public class TrainingDummyDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    public TrainingDummyDeckBuilder(int size) : base(size)
    {
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = GetCloneOfTrainingDummyCard();
        }

        return deck;
    }
}
