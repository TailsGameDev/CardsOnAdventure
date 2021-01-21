using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsCollection : CardPrototypesAccessor
{
    public static void AddOneOfEachCardOfClassToCollection(Classes classe)
    {
        for (int c = 0; c < allCardPrototypes.Length; c++)
        {
            Card card = allCardPrototypes[c];
            if (card.Classe == classe)
            {
                SumToCurrentAmount(card, 1);
            }
        }
    }
    public static void SumToCurrentAmount(Card card, int amountToAdd)
    {
        int c = GetIndexInAllCardPrototypesArray(card);

        int[] cardsCollectionAmounts = GetCardsCollectionAmounts();
        // c+1 because the random card index is 0.
        cardsCollectionAmounts[c] += amountToAdd;
        saveFacade.PrepareCardsCollectionForSaving(new DeckSerializable(cardsCollectionAmounts));
    }
    private static int GetIndexInAllCardPrototypesArray(Card card)
    {
        int c = -1;
        for (int i = 0; i < allCardPrototypes.Length; i++)
        {
            if (card.IsAnotherInstanceOf(allCardPrototypes[i]))
            {
                c = i;
                break;
            }
        }
        return c;
    }
    public static int[] GetCardsCollectionAmounts()
    {
        int[] cardAmounts;
        if (saveFacade.IsCardsCollectionLoaded())
        {
            cardAmounts = saveFacade.GetLoadedCardsCollection().cardsIndexes;
        }
        else
        {
            cardAmounts = new int[allCardPrototypes.Length];
            for (int i = 0; i < cardAmounts.Length; i++)
            {
                // For now, let's consider the collection starts with one card of each type.
                // Maybe this change later when unlock new types of card, maybe not!
                cardAmounts[i] = 1;
            }
        }
        return cardAmounts;
    }

    public static Card[] GetClonesOfCardsOnPlayerCollection()
    {
        return OneOfEachButNoMonstersDeckBuilder.Create().GetDeck();
    }
}
