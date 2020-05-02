using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    private static DeckBuilder enemyDeckMounter;
    private static DeckBuilder playerDeckBuilder;

    [SerializeField]
    private int defaultDeckSize = -1;

    protected Card[] allCardPrototypes;

    [SerializeField]
    protected Card theRandomCard;

    protected const int NOT_A_SIZE = -1;
    protected const int TOUGH_SIZE = -2;
    protected const int BOSS_SIZE = -3;

    public static int DefaultDeckSize { get => deckPrototypeFactory.defaultDeckSize; }

    private void Awake()
    {
        BecomeSingleton();
    }

    private void Start()
    {
        PopulateArrayOfAllCardPrototypes();
        // Useful when play from Battle scene.
        MakeDecksRandomIfTheyAreNull();
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

    private void MakeDecksRandomIfTheyAreNull()
    {
        if (enemyDeckMounter == null)
        {
            enemyDeckMounter = new RandomDeckMounter(defaultDeckSize);
        }

        if (playerDeckBuilder == null)
        {
            playerDeckBuilder = new RandomDeckMounter(defaultDeckSize);
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

    public static Card GetCloneOfTheRandomCard()
    {
        return Instantiate(deckPrototypeFactory.theRandomCard);
    }

    public static Card[] GetPreparedCardsForTheEnemy()
    {
        return enemyDeckMounter.GetDeck();
    }

    public static Card[] GetPreparedCardsForThePlayerOrGetRandomDeck()
    {
        if (playerDeckBuilder == null)
        {
            PrepareRandomDeckForThePlayer();
        }

        Card[] playerDeck = playerDeckBuilder.GetDeck();

        DeckBuilder.Shuffle(ref playerDeck);

        return ReplaceRandomAndSumBonuses(playerDeck);
    }

    private static Card[] ReplaceRandomAndSumBonuses(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            // Replace Random
            if (playerDeck[i].IsAnotherInstanceOf( deckPrototypeFactory.theRandomCard) )
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCardFromPrototypesRandomly();
            }

            // Sum bonuses
            playerDeck[i].ApplyPlayerBonuses();
        }

        return playerDeck;
    }

    private static Card GetCardFromPrototypesRandomly()
    {
        Card[] prototypes = deckPrototypeFactory.allCardPrototypes;
        int randomIndex = UnityEngine.Random.Range(0, prototypes.Length);
        return prototypes[randomIndex];
    }

    public static Card[] GetCopyOfAllAndEachCardPrototypePlusTheRandomCard()
    {
        return OneOfEachAndOneRandomCardDeckBuilder.Create().GetDeck();
    }

    #region Public Prepare XXXX Deck For The Enemy
    public static void PrepareRandomDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new RandomDeckMounter(size);
    }
    public static void PrepareToughRandomDeckForTheEnemy(int size = TOUGH_SIZE)
    {
        enemyDeckMounter = new RandomDeckMounter(size);
    }
    public static void PrepareBossRandomDeckForTheEnemy(int size = BOSS_SIZE)
    {
        enemyDeckMounter = new RandomDeckMounter(size);
    }

    public static void PrepareMageDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.MAGE);
    }
    public static void PrepareWarriorDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.WARRIOR);
    }
    public static void PrepareRogueDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.ROGUE);
    }
    public static void PrepareGuardianDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.GUARDIAN);
    }
    #endregion

    public static void PrepareRandomDeckForThePlayer(int size = NOT_A_SIZE)
    {
        playerDeckBuilder = new RandomDeckMounter(size);
    }

    public static void PrepareManuallyBuiltDeckForThePlayer(Card[] cards)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cards);
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

