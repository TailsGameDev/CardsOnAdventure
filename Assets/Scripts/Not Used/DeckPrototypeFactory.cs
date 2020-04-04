using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory battleSceneDeckFactory;

    private static DeckMounter enemyDeckMounter;
    private static DeckMounter playerDeckMounter;

    [SerializeField]
    protected int defaultDeckSize;

    private int difficultyLevel = 1;

    [SerializeField]
    protected Card[] mages = null;
    [SerializeField]
    protected Card[] warriors = null;
    [SerializeField]
    protected Card[] rogues = null;
    [SerializeField]
    protected Card[] guardians = null;
    [SerializeField]
    protected Card[] clerics = null;

    protected Card[] allCardPrototypes;

    protected const int NOT_A_SIZE = -1;
    protected const int TOUGH_SIZE = -2;

    private void Awake()
    {
        BecomeSingleton();

        MakeDecksRandomIfTheyAreNull();

        PopulateArrayOfAllCardPrototypes();
    }

    private void BecomeSingleton()
    {
        if (battleSceneDeckFactory == null)
        {
            battleSceneDeckFactory = this;
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
        List<Card> allCardPrototypesList = new List<Card>();

        allCardPrototypesList.AddRange(mages);
        allCardPrototypesList.AddRange(warriors);
        allCardPrototypesList.AddRange(rogues);
        allCardPrototypesList.AddRange(guardians);
        allCardPrototypesList.AddRange(clerics);

        this.allCardPrototypes = allCardPrototypesList.ToArray();
    }

    private void OnDestroy()
    {
        battleSceneDeckFactory = null;
    }

    public static Card[] GetPreparedCardsForTheEnemy()
    {
        Card[] enemyDeck = enemyDeckMounter.GetDeck();

        for (int i = 0; i < enemyDeck.Length; i++)
        {
            enemyDeck[i].AjustCardToDifficult(battleSceneDeckFactory.difficultyLevel);
        }

        return enemyDeck;
    }

    public static Card[] GetPreparedCardsForThePlayer()
    {
        return playerDeckMounter.GetDeck();   
    }

    public static void SetDifficultyLevelForEnemyDeck(int difficultyLevel)
    {
        if (difficultyLevel < 1)
        {
            battleSceneDeckFactory.difficultyLevel = 1;
        }
        else
        {
            battleSceneDeckFactory.difficultyLevel = difficultyLevel;
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

    public static void PrepareMageDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.Mage);
    }
    public static void PrepareWarriorDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.Warrior);
    }
    public static void PrepareRogueDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.Rogue);
    }
    public static void PrepareGuardianDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.Guardian);
    }
    public static void PrepareClericDeckForTheEnemy(int size = NOT_A_SIZE)
    {
        enemyDeckMounter = new HalfRandomDeckMounter(size, Classes.Cleric);
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

        protected void InitializeDeck()
        {
            if (size == NOT_A_SIZE)
            {
                size = battleSceneDeckFactory.defaultDeckSize;
            } 
            else if ( size == TOUGH_SIZE)
            {
                size = battleSceneDeckFactory.defaultDeckSize + 1;
            }

            deck = new Card[size];
        }

        protected Card[] BuildHalfRandomDeck(Card[] notRandomPartPrototypes)
        {
            deck = InOrderBuildRangeWithPrototypes(beginningIndex: 0, limitIndex: size/2, notRandomPartPrototypes);

            Card[] allPrototypesThereAre = battleSceneDeckFactory.allCardPrototypes;
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
                int random = Random.Range(0, prototypes.Length);
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

    public class RandomDeckMounter : DeckMounter
    {
        public RandomDeckMounter(int size) : base(size)
        {
        }

        public override Card[] GetDeck()
        {
            InitializeDeck();

            Card[] prototypes = battleSceneDeckFactory.allCardPrototypes;

            deck = BuildFullRandomDeckFromPrototypes(battleSceneDeckFactory.allCardPrototypes);

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
            InitializeDeck();

            Card[] notRandomPartPrototypes;

            /*
             It would be cool to receive the notRandomPartPrototypes as an argument in the constructor, but
             the cards does not exist in other scenes, just in the battle... and currently you don't need
             to be in the battle scene to look to the map.
             */

            switch (classe)
            {
                case Classes.Mage:
                    notRandomPartPrototypes = battleSceneDeckFactory.mages;
                    break;
                case Classes.Warrior:
                    notRandomPartPrototypes = battleSceneDeckFactory.warriors;
                    break;
                case Classes.Rogue:
                    notRandomPartPrototypes = battleSceneDeckFactory.rogues;
                    break;
                case Classes.Guardian:
                    notRandomPartPrototypes = battleSceneDeckFactory.guardians;
                    break;
                case Classes.Cleric:
                    notRandomPartPrototypes = battleSceneDeckFactory.clerics;
                    break;
                default:
                    notRandomPartPrototypes = battleSceneDeckFactory.allCardPrototypes;
                    break;
            }

            return BuildHalfRandomDeck(notRandomPartPrototypes);
        }
    }
    #endregion
}


