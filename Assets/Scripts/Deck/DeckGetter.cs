using System.Collections.Generic;
using UnityEngine;

public class DeckGetter : CardPrototypesAccessor
{
    private static DeckPrototypeFactory.DeckBuilder enemyDeckBuilder;
    private static DeckPrototypeFactory.DeckBuilder playerDeckBuilder;

    private static SaveFacade saveFacade = new SaveFacade();

    public static void PrepareEditorMadeDeckForTheEnemy(string deckName)
    {
        enemyDeckBuilder = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder(deckName);
    }
    public static Card[] GetPreparedCardsForTheEnemy()
    {
        if (enemyDeckBuilder == null)
        {
            enemyDeckBuilder = new RandomDeckBuilder(DeckPrototypeFactory.DefaultDeckSize);
        }

        Card[] cards = ReplaceTheRandomCards(enemyDeckBuilder.GetDeck());
        if (MapScroller.GetMapLevel() != 1)
        {
            // Level Up 2 times
            for (int c = 0; c < cards.Length; c++)
            {
                cards[c].ApplyLevelBonus(2);
            }
        }

        return cards;
    }

    #region Prepare Player's Deck
    public static void PrepareFirstDeckIfNeededForThePlayerAndGetReadyForSaving(bool forceToPrepare = false)
    {
        if (playerDeckBuilder == null || forceToPrepare)
        {
            playerDeckBuilder = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder("PlayerDeck");
            int[] cardIndexes = ((EditorMadeDeckBuilder)playerDeckBuilder).GetIndexOfEachCardPrototype();
            PrepareDeckIndexesForSaving(cardIndexes);
        }
    }
    public static void PrepareManuallyBuiltDeckForThePlayerAndGetReadyForSaving(Card[] cards)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cards);

        int[] cardIndexes = ((ManualDeckBuider)playerDeckBuilder).GetIndexOfEachCardPrototype();
        PrepareDeckIndexesForSaving(cardIndexes);
    }
    private static void PrepareDeckIndexesForSaving(int[] deckIndexes)
    {
        DeckSerializable deckSerializable = new DeckSerializable(deckIndexes);
        saveFacade.PrepareDeckForSaving(deckSerializable);
    }
    public static void PrepareLoadedDeckForThePlayer(int[] cardIndexes)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cardIndexes);
    }
    #endregion

    #region Get Player's Deck
    public static Card[] GetPreparedCardsForThePlayerOrGetRandomDeck()
    {
        Card[] playerDeck = GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses();

        DeckPrototypeFactory.DeckBuilder.Shuffle(ref playerDeck);

        playerDeck = ReplaceTheRandomCards(playerDeck);

        playerDeck = ReplaceMonsters(playerDeck);

        return ApplyPlayerBonuses(playerDeck);
    }
    private static Card[] GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses()
    {
        Card[] playerDeck;

        if (saveFacade.IsDeckLoaded())
        {
            DeckSerializable deckSerializable = saveFacade.GetLoadedDeck();
            PrepareLoadedDeckForThePlayer(deckSerializable.GetCardsIndexes());
            playerDeck = playerDeckBuilder.GetDeck();
        }
        else
        {
            PrepareFirstDeckIfNeededForThePlayerAndGetReadyForSaving();
            playerDeck = playerDeckBuilder.GetDeck();
        }

        return playerDeck;
    }
    private static Card[] ReplaceTheRandomCards(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i].IsAnotherInstanceOf(CardPrototypesAccessor.theRandomCardPrototype))
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneOfCardFromPrototypesRandomlyButNotTheTrainingDummy();
            }
        }

        return playerDeck;
    }
    private static Card GetCloneOfCardFromPrototypesRandomlyButNotTheTrainingDummy()
    {
        Card[] prototypes = CardPrototypesAccessor.allCardPrototypes;
        int randomIndex = Random.Range(0, prototypes.Length);
        return prototypes[randomIndex].GetClone();
    }
    private static Card[] ReplaceMonsters(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i].Classe == Classes.MONSTER)
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneFromNotMonsterPrototypesRandomly();
            }
        }

        return playerDeck;
    }
    private static Card GetCloneFromNotMonsterPrototypesRandomly()
    {
        List<Card> prototypes = CardPrototypesAccessor.notMonsterPrototypes;
        int randomIndex = UnityEngine.Random.Range(0, prototypes.Count);
        return prototypes[randomIndex].GetClone();
    }
    private static Card[] ApplyPlayerBonuses(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            playerDeck[i].ApplyPlayerBonuses();
        }

        return playerDeck;
    }

    public static Card[] GetPreparedCardsForThePlayerWithTheRandomCards()
    {
        Card[] playerDeck = GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses();

        return ApplyPlayerBonuses(playerDeck);
    }
    #endregion

    // TODO: Make DeckBuilder for this
    public static Card[] Get2DifferentRandomCards(int deckSize)
    {
        Card[] cards = new Card[2];

        cards[0] = GetCloneFromNotMonsterPrototypesRandomly();

        do
        {
            cards[1] = GetCloneFromNotMonsterPrototypesRandomly();
        } while ( cards[0].IsAnotherInstanceOf(cards[1]) );

        return cards;
    }
}
