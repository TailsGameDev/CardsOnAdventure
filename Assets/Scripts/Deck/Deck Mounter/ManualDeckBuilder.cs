using UnityEngine;
public class ManualDeckBuider : DeckPrototypeFactory.DeckBuilder
{
    private int[] indexOfEachCardPrototype;

    private ManualDeckBuider(int size) : base(size)
    {
        indexOfEachCardPrototype = new int[size];
    }

    public static ManualDeckBuider Create(Card[] cardsToBeOnDeck)
    {
        ManualDeckBuider builder = new ManualDeckBuider(cardsToBeOnDeck.Length);

        builder.FindAndSaveThePrototypeIndexForEachCard(cardsToBeOnDeck);

        /* funcionando...
        for (int i = 0; i < builder.indexOfEachCardPrototype.Length; i++)
        {
            Debug.Log(builder.indexOfEachCardPrototype[i]);
        }
        */

        return builder;
    }

    private void FindAndSaveThePrototypeIndexForEachCard(Card[] cardsToBeOnDeck)
    {
        for (int i = 0; i < size; i++)
        {
            // Find
            Card cardOfDeck = cardsToBeOnDeck[i];
            int prototypeIndex = FindIndexOnPrototypesArray(cardOfDeck);
            // Save
            indexOfEachCardPrototype[i] = prototypeIndex;
        }
    }

    private int FindIndexOnPrototypesArray(Card card)
    {
        int prototypeIndex = -1;
        for (int iterationIndex = 0; iterationIndex < allCardPrototypes.Length; iterationIndex++)
        {
            Card prototype = allCardPrototypes[iterationIndex];
            if (card.IsAnotherInstanceOf(prototype))
            {
                prototypeIndex = iterationIndex;
                break;
            }
        }
        return prototypeIndex;
    }

    public override Card[] GetDeck()
    {
        deck = new Card[size];

        for (int i = 0; i < size; i++)
        {
            int prototypeIndex = indexOfEachCardPrototype[i];
            Card card;
            if (prototypeIndex != -1)
            {
                card = allCardPrototypes[prototypeIndex].GetClone();
            }
            else
            {
                card = GetCloneOfTheRandomCard();
            }
            deck[i] = card;
        }

        return deck;
    }
}
