using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsCollection : CardPrototypesAccessor
{
    public static void AddOneOfEachUnblockedCardOfClassToCollection(Classes classe)
    {
        for (int c = 0; c < allCardPrototypes.Length; c++)
        {
            Card card = allCardPrototypes[c];
            if (card.Classe == classe && !IsCardLocked(c))
            {
                SumToCurrentAmount(card, 1);
            }
        }
    }
    public static void SumToCurrentAmount(Card card, int amountToAdd)
    {
        int c = GetIndexInAllCardPrototypesArray(card);

        int[] cardsCollectionAmounts = GetCardsCollectionAmounts();

        if (IsCardLocked(c))
        {
            // That operation also unlocks the card.
            cardsCollectionAmounts[c] = amountToAdd;
        }
        else
        {
            cardsCollectionAmounts[c] += amountToAdd;
        }
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
            cardAmounts = saveFacade.GetLoadedCardsCollection().cardAmounts;
        }
        else
        {
            cardAmounts = new int[allCardPrototypes.Length];
            for (int i = 0; i < cardAmounts.Length; i++)
            {
                cardAmounts[i] = allCardPrototypes[i].InitialAmountOnCollection;
            }
        }
        return cardAmounts;
    }

    public static bool IsCardLocked(Card card)
    {
        int index = FindIndexOnPrototypesArray(card);
        return IsCardLocked(index);
    }
    public static bool IsCardLocked(int cardIndexOnAllPrototypesArray)
    {
        int[] cardAmounts = GetCardsCollectionAmounts();
        bool isBlocked = cardAmounts[cardIndexOnAllPrototypesArray] < 0;
        return isBlocked;
    }

    public static Card[] GetUnlockedCardsFrom(Card[] arrayToFilter)
    {
        int amountOfUnlockedCards = 0;
        
        for (int c = 0; c < arrayToFilter.Length; c++)
        {
            if ( ! arrayToFilter[c].IsLocked())
            {
                amountOfUnlockedCards++;
            }
        }
        Card[] onlyUnlockedCards = new Card[amountOfUnlockedCards];
        int unlockedIndex = 0;
        for (int c = 0; c < arrayToFilter.Length; c++)
        {
            if (!arrayToFilter[c].IsLocked())
            {
                onlyUnlockedCards[unlockedIndex] = arrayToFilter[c];
                unlockedIndex++;
            }
        }
        return onlyUnlockedCards;
    }

    public static Card[] GetClonesOfCardsOnPlayerCollection()
    {
        return OneOfEachButNoMonstersDeckBuilder.Create().GetDeck();
    }
}
