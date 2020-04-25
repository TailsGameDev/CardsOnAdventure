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

    public abstract class DeckMounter
    {
        protected int size;
        protected Card[] deck;
        protected readonly Card[] allCardPrototypes;

        public DeckMounter(int size)
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
                deck[i] = Instantiate(prototypes[i % prototypes.Length]);
            }

            return deck;
        }

        protected Card[] OutOfOrderBuildRangeWithPrototypes(int beginningIndex, int limitIndex, Card[] prototypes)
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

        /// <summary>
        /// Instantiate a card. The reason not to just use Instantiate is 'DeckMounter' does not inherits MonoBehavior.
        /// So DeckMounter can access 'Instantiate' because it is an internal class of DeckPrototypeFactory.
        /// </summary>
        protected Card InstantiatePlease(Card card)
        {
            return Instantiate(card);
        }
    }
}


