using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoMonstersDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    public NoMonstersDeckBuilder(int size) : base(size)
    {
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            int r = Random.Range(0, notMonsterPrototypes.Count);
            deck[i] = notMonsterPrototypes[r].GetClone();
        }

        return deck;
    }
}
