using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FullClassDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    Classes classe;

    public FullClassDeckBuilder(int size, Classes classe) : base(size)
    {
        this.classe = classe;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        List<Card> prototypesOfClass = new List<Card>();

        for (int i = 0; i < allCardPrototypes.Length; i++)
        {
            if (allCardPrototypes[i].Classe == classe)
            {
                prototypesOfClass.Add(allCardPrototypes[i]);
            }
        }

        for (int k = 0; k < deck.Length; k++)
        {
            int r = Random.Range(0, prototypesOfClass.Count);
            deck[k] = prototypesOfClass[r].GetClone();
        }

        return deck;
    }
}
