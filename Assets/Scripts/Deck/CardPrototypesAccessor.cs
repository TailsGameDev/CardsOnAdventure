using System.Collections.Generic;
using UnityEngine;

public class CardPrototypesAccessor
{
    // Contains all cards except theRandomCard and trainingDummy
    protected static Card[] allCardPrototypes;

    // Note notMonsterPrototypes are a subset of allCardPrototypes
    protected static List<Card> notMonsterPrototypes;

    // Note the random card is not on the allCardPrototypes array
    protected static Card theRandomCardPrototype;

    protected static SaveFacade saveFacade = new SaveFacade();

    public static int FindIndexOnPrototypesArray(Card card)
    {
        // -1 can be interpreted as the Random Card
        int prototypeIndex = -1;
        Card[] allCardPrototypes = CardPrototypesAccessor.allCardPrototypes;
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

    public int GetAmountOfCardPrototypes()
    {
        return allCardPrototypes.Length;
    }

    public static void UpdatePrototypesLevel()
    {
        for (int p = 0; p < allCardPrototypes.Length; p++)
        {
            allCardPrototypes[p].RefreshStatsForThePlayer();
        }
    }
}
