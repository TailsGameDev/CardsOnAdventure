using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    private static DeckMounter enemyDeckMounter;
    private static DeckMounter playerDeckMounter;

    [SerializeField]
    protected int defaultDeckSize;

    private int difficultyLevel = 1;

    protected Card[] allCardPrototypes;

    protected const int NOT_A_SIZE = -1;
    protected const int TOUGH_SIZE = -2;
    protected const int BOSS_SIZE = -3;

    private void Awake()
    {
        BecomeSingleton();

        MakeDecksRandomIfTheyAreNull();
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

    private void MakeDecksRandomIfTheyAreNull()
    {
        if (enemyDeckMounter == null)
        {
            enemyDeckMounter = new RandomDeckMounter(defaultDeckSize);
        }

        if (playerDeckMounter == null)
        {
            playerDeckMounter = new RandomDeckMounter(defaultDeckSize);
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

    public static Card[] GetPreparedCardsForTheEnemy()
    {
        Card[] enemyDeck = enemyDeckMounter.GetDeck();

        for (int i = 0; i < enemyDeck.Length; i++)
        {
            enemyDeck[i].AjustCardToDifficult(deckPrototypeFactory.difficultyLevel);
        }

        return enemyDeck;
    }

    public static Card[] GetPreparedCardsForThePlayer()
    {
        Card[] playerDeck = playerDeckMounter.GetDeck();

        for (int i = 0; i < playerDeck.Length; i++)
        {
            playerDeck[i].SumPlayerBonuses();
        }

        return playerDeck;
    }

    public static void SetDifficultyLevelForEnemyDeck(int difficultyLevel)
    {
        if (difficultyLevel < 1)
        {
            deckPrototypeFactory.difficultyLevel = 1;
        }
        else
        {
            deckPrototypeFactory.difficultyLevel = difficultyLevel;
        }
    }

    #region Prepare XXXX Deck For The Enemy

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
    public static void PrepareClericDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.CLERIC);
    }

    #endregion

    public static void PrepareRandomDeckForThePlayer(int size = NOT_A_SIZE)
    {
        playerDeckMounter = new RandomDeckMounter(size);
    }

    #region Deck Mounters

    public abstract class DeckMounter
    {
        protected int size;
        protected Card[] deck;

        public DeckMounter(int size)
        {
            this.size = size;
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

            deck = new Card[size];
        }

        protected Card[] BuildHalfRandomDeck(Card[] notRandomPartPrototypes)
        {
            deck = InOrderBuildRangeWithPrototypes(beginningIndex: 0, limitIndex: size/2, notRandomPartPrototypes);

            Card[] allPrototypesThereAre = deckPrototypeFactory.allCardPrototypes;
            deck = OutOfOrderBuildRangeWithPrototypes(beginningIndex: size / 2, limitIndex: size, allPrototypesThereAre);

            Shuffle(ref deck);

            return deck;
        }

        protected Card[] BuildFullRandomDeckFromPrototypes(Card[] prototypes)
        {
            deck = OutOfOrderBuildRangeWithPrototypes(beginningIndex: 0, limitIndex: size, prototypes);

            return deck;
        }

        private Card[] InOrderBuildRangeWithPrototypes(int beginningIndex, int limitIndex, Card[] prototypes)
        {
            for (int i = beginningIndex; i < limitIndex; i++)
            {
                deck[i] = Instantiate(prototypes[i % prototypes.Length]);
            }

            return deck;
        }

        private Card[] OutOfOrderBuildRangeWithPrototypes(int beginningIndex, int limitIndex, Card[] prototypes)
        {
            for (int i = beginningIndex; i < limitIndex; i++)
            {
                int random = UnityEngine.Random.Range(0, prototypes.Length);
                deck[i] = Instantiate(prototypes[random]);
            }

            return deck;
        }

        protected static void Shuffle<T>(ref T[] array)
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
    }

    public class OneOfEachCardDeckMounter : DeckMounter
    {
        private OneOfEachCardDeckMounter(int size) : base (size)
        {
        }

        public static OneOfEachCardDeckMounter New()
        {
            return new OneOfEachCardDeckMounter(deckPrototypeFactory.allCardPrototypes.Length);
        }

        public override Card[] GetDeck()
        {
            deck = new Card[size];

            for (int i = 0; i < deck.Length; i++)
            {
                deck[i] = Instantiate(deckPrototypeFactory.allCardPrototypes[i]);
            }

            return deck;
        }
    }

    public class RandomDeckMounter : DeckMounter
    {
        public RandomDeckMounter(int size) : base(size)
        {
        }

        public override Card[] GetDeck()
        {
            CreateEmptyDeckWithProperSize();

            Card[] prototypes = deckPrototypeFactory.allCardPrototypes;

            deck = BuildFullRandomDeckFromPrototypes(deckPrototypeFactory.allCardPrototypes);

            return deck;
        }
    }

    public class HalfRandomDeckMounter : DeckMounter
    {
        private Classes classe;

        public HalfRandomDeckMounter(int size, Classes classe) : base(size)
        {
            this.classe = classe;
        }

        public override Card[] GetDeck()
        {
            CreateEmptyDeckWithProperSize();

            Card[] notRandomPartPrototypes = ClassInfo.GetCardsOfClass(classe);

            if (notRandomPartPrototypes == null || notRandomPartPrototypes.Length == 0)
            {
                Debug.LogError("[DeckPrototypeFactory] notRandomPartPrototypes.Length == 0. "+
                                "It should be the size of the available cards of a class.");
            }

            return BuildHalfRandomDeck(notRandomPartPrototypes);
        }
    }
    #endregion
}


