using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    private static DeckBuilder enemyDeckBuilder;
    private static DeckBuilder playerDeckBuilder;

    [SerializeField]
    private int defaultDeckSize = -1;

    protected Card[] allCardPrototypes;

    [SerializeField]
    protected Card theRandomCard;

    protected const int NOT_A_SIZE = -1;
    protected const int TOUGH_SIZE = -2;
    protected const int BOSS_SIZE = -3;

    private static SaveFacade saveFacade = new SaveFacade();

    public static int DefaultDeckSize { get => deckPrototypeFactory.defaultDeckSize; }

    #region Initialization
    private void Awake()
    {
        BecomeSingleton();
    }

    private void Start()
    {
        PopulateArrayOfAllCardPrototypes();
    }

    private void BecomeSingleton()
    {
        if (deckPrototypeFactory == null)
        {
            deckPrototypeFactory = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PopulateArrayOfAllCardPrototypes()
    {
        Classes[] classes = (Classes[]) System.Enum.GetValues(typeof(Classes));

        List<Card> allCardPrototypesList = new List<Card>();

        // Starts at 1 so it won't take the Classes.NOT_A_CLASS key
        for (int i = 1; i < classes.Length; i++)
        {
            Card[] cardsOfClass = ClassInfo.GetCardsOfClass(classes[i]);
            allCardPrototypesList.AddRange(cardsOfClass);
        }

        allCardPrototypes = allCardPrototypesList.ToArray();
    }
    #endregion

    public static Card GetCloneOfTheRandomCard()
    {
        return Instantiate(deckPrototypeFactory.theRandomCard);
    }

    public static Card[] GetCopyOfAllAndEachCardPrototypePlusTheRandomCard()
    {
        return OneOfEachAndOneRandomCardDeckBuilder.Create().GetDeck();
    }

    public static Card[] GetPreparedCardsForTheEnemy()
    {
        if (enemyDeckBuilder == null)
        {
            enemyDeckBuilder = new RandomDeckBuilder(DefaultDeckSize);
        }
        return enemyDeckBuilder.GetDeck();
    }
    #region Public Prepare XXXX Deck For The Enemy
    public static void PrepareRandomDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckBuilder = new RandomDeckBuilder(size);
    }
    public static void PrepareToughRandomDeckForTheEnemy(int size = TOUGH_SIZE)
    {
        enemyDeckBuilder = new RandomDeckBuilder(size);
    }
    public static void PrepareBossRandomDeckForTheEnemy(int size = BOSS_SIZE)
    {
        enemyDeckBuilder = new RandomDeckBuilder(size);
    }
    public static void PrepareModifiedSizeRandomDeckForTheEnemy(float sizeMultiplier)
    {
        enemyDeckBuilder = new RandomDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier));
    }

    public static void PrepareMageDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(size, Classes.MAGE);
    }
    public static void PrepareWarriorDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(size, Classes.WARRIOR);
    }
    public static void PrepareRogueDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(size, Classes.ROGUE);
    }
    public static void PrepareGuardianDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(size, Classes.GUARDIAN);
    }
    public static void PrepareClassDeckForTheEnemy(float sizeMultiplier, Classes classe)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier), classe);
    }
    #endregion

    // Player's Deck
    public static Card[] GetPreparedCardsForThePlayerOrGetRandomDeck()
    {
        if (playerDeckBuilder == null)
        {
            if ( saveFacade.IsDeckLoaded() )
            {
                DeckSerializable deckSerializable = saveFacade.GetLoadedDeck();
                PrepareLoadedDeckForThePlayer(deckSerializable.GetCardsIndexes());
            }
            else
            {
                PrepareRandomDeckForThePlayerAndSaveItInStorage();
            }
        }

        Card[] playerDeck = playerDeckBuilder.GetDeck();

        DeckBuilder.Shuffle(ref playerDeck);

        return ReplaceRandomAndSumBonuses(playerDeck);
    }
    public static void PrepareRandomDeckForThePlayerAndSaveItInStorage(int size = NOT_A_SIZE)
    {
        playerDeckBuilder = new RandomDeckBuilder(size);

        int adjustedSize;
        if (size == NOT_A_SIZE)
        {
            adjustedSize = DefaultDeckSize;
        }
        else
        {
            adjustedSize = size;
        }

        int[] cardIndexes = GetDefaultIndexes(adjustedSize);
        SaveIndexesInStorage(cardIndexes);
    }
    public static int[] GetDefaultIndexes()
    {
        return GetDefaultIndexes(DefaultDeckSize);
    }
    public static int[] GetDefaultIndexes(int adjustedSize)
    {
        int[] cardIndexes = new int[adjustedSize];
        for (int i = 0; i < adjustedSize; i++)
        {
            cardIndexes[i] = ManualDeckBuider.INDEX_OF_RANDOM_CARD;
        }
        return cardIndexes;
    }
    private static void SaveIndexesInStorage(int[] cardIndexes)
    {
        DeckSerializable deckSerializable = new DeckSerializable(cardIndexes);
        saveFacade.PrepareDeckForSaving(deckSerializable);
    }
    public static void PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(Card[] cards)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cards);

        int[] cardIndexes = ((ManualDeckBuider)playerDeckBuilder).GetIndexesOfEachCardPrototype();
        SaveIndexesInStorage(cardIndexes);
    }
    public static void PrepareLoadedDeckForThePlayer(int[] cardIndexes)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cardIndexes);
    }
    private static Card[] ReplaceRandomAndSumBonuses(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            // Replace Random
            if (playerDeck[i].IsAnotherInstanceOf( deckPrototypeFactory.theRandomCard) )
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneOfCardFromPrototypesRandomly();
            }

            // Sum bonuses
            playerDeck[i].ApplyPlayerBonuses();
        }

        return playerDeck;
    }
    private static Card GetCloneOfCardFromPrototypesRandomly()
    {
        Card[] prototypes = deckPrototypeFactory.allCardPrototypes;
        int randomIndex = UnityEngine.Random.Range(0, prototypes.Length);
        return prototypes[randomIndex].GetClone();
    }

    public abstract class DeckBuilder
    {
        protected int size;
        protected Card[] deck;
        protected readonly Card[] allCardPrototypes;

        public DeckBuilder(int size)
        {
            this.size = size;
            allCardPrototypes = deckPrototypeFactory.allCardPrototypes;
        }

        public abstract Card[] GetDeck();

        protected void CreateEmptyDeckWithProperSize()
        {
            if (size == NOT_A_SIZE)
            {
                size = deckPrototypeFactory.defaultDeckSize;
            } 
            else if ( size == TOUGH_SIZE)
            {
                size = deckPrototypeFactory.defaultDeckSize + 1;
            }
            else if (size == BOSS_SIZE)
            {
                size = deckPrototypeFactory.defaultDeckSize + 3;
            }
            // Else: keep the size it was before!

            deck = new Card[size];
        }

        protected Card[] InOrderBuildRangeWithPrototypes(int beginningIndex, int limitIndex, Card[] prototypes)
        {
            for (int i = beginningIndex; i < limitIndex; i++)
            {
                deck[i] = prototypes[i % prototypes.Length].GetClone();
            }

            return deck;
        }

        protected Card[] OutOfOrderBuildRangeWithPrototypes(int beginningIndex, int limitIndex, Card[] prototypes)
        {
            for (int i = beginningIndex; i < limitIndex; i++)
            {
                int random = UnityEngine.Random.Range(0, prototypes.Length);
                deck[i] = prototypes[random].GetClone();
            }

            return deck;
        }

        public static void Shuffle<T>(ref T[] array)
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

        protected Card GetCloneOfTheRandomCard()
        {
            return deckPrototypeFactory.theRandomCard.GetClone();
        }
    }
}

