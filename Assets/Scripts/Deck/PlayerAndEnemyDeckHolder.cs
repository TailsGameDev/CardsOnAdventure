using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAndEnemyDeckHolder : CardPrototypesAccessor
{
    private static DeckBuilderSuperclass enemyDeckBuilder;
    private static DeckBuilderSuperclass playerDeckBuilder;

    public static void PrepareEditorMadeDeckForTheEnemy(string deckName)
    {
        enemyDeckBuilder = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder(deckName);
    }
    public static Card[] GetPreparedCardsForTheEnemy()
    {
        if (enemyDeckBuilder == null)
        {
            enemyDeckBuilder = new RandomDeckBuilder(DeckBuilderSuperclass.DEFAULT_DECK_SIZE);
        }

        // Apply Map bonus
        Card[] cards = ReplaceTheRandomCards(enemyDeckBuilder.GetDeck());
        if (MapScroller.GetMapLevel() != 1)
        {
            // Level Up 2 times
            for (int c = 0; c < cards.Length; c++)
            {
                cards[c].SumLevelBonus(2);
            }
        }

        // Apply Spot Bonus
        int spotLevel = MapsCache.SpotToClearAndLevelUpIfPlayerWins.Level;
        if (spotLevel > 0)
        {
            for (int c = 0; c < cards.Length; c++)
            {
                cards[c].SumLevelBonus(spotLevel);
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

        Shuffle(ref playerDeck);

        playerDeck = ReplaceTheRandomCards(playerDeck);

        playerDeck = ReplaceMonsters(playerDeck);

        return RefreshStatsForThePlayer(playerDeck);
    }
    private static void Shuffle<T>(ref T[] array)
    {
        System.Random rng = new System.Random();

        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
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
                Object.Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneOfCardFromAvailablePrototypesRandomly();
            }
        }

        return playerDeck;
    }
    private static Card GetCloneOfCardFromAvailablePrototypesRandomly()
    {
        Card[] allPrototypes = CardPrototypesAccessor.allCardPrototypes;
        Card[] unlockedCards = CardsCollection.GetUnlockedCardsFrom(allPrototypes);
        int randomIndex = Random.Range(0, unlockedCards.Length);
        return unlockedCards[randomIndex].GetClone();
    }
    private static Card[] ReplaceMonsters(Card[] playerDeck)
    {
        Card[] notMonstersUnlocked = CardsCollection.GetUnlockedCardsFrom(CardPrototypesAccessor.notMonsterPrototypes);
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i].Classe == Classes.MONSTER)
            {
                Object.Destroy(playerDeck[i].gameObject);

                int randomIndex = UnityEngine.Random.Range(0, notMonstersUnlocked.Length);
                playerDeck[i] = notMonstersUnlocked[randomIndex].GetClone();
            }
        }

        return playerDeck;
    }
    private static Card[] RefreshStatsForThePlayer(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            playerDeck[i].RefreshStatsForThePlayer();
        }

        return playerDeck;
    }

    public static Card[] GetPreparedCardsForThePlayerWithTheRandomCards()
    {
        Card[] playerDeck = GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses();

        return RefreshStatsForThePlayer(playerDeck);
    }
    #endregion

    // TODO: Make DeckBuilder for this
    public static Card[] Get2DifferentRandomUnlockedNotMonsterCards(int deckSize)
    {
        Card[] notMonstersUnlocked = CardsCollection.GetUnlockedCardsFrom(CardPrototypesAccessor.notMonsterPrototypes);
        Card[] cards = new Card[2];

        cards[0] = notMonstersUnlocked[ Random.Range(0, notMonstersUnlocked.Length) ].GetClone();

        do
        {
            cards[1] = notMonstersUnlocked[Random.Range(0, notMonstersUnlocked.Length)].GetClone();
        } while ( cards[0].IsAnotherInstanceOf(cards[1]) );

        return cards;
    }
}
