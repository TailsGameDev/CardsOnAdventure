using System.Collections.Generic;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    private static DeckBuilder enemyDeckBuilder;
    private static DeckBuilder playerDeckBuilder;
    private static DeckBuilder trainingDeckForThePlayer;

    [SerializeField]
    private int defaultDeckSize = -1;

    protected Card[] allCardPrototypes;
    protected List<Card> notMonsterPrototypes;

    [SerializeField]
    protected Card theRandomCard;
    [SerializeField]
    protected Card trainingDummyCard;

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
        PopulateArrayOfNotMonsterPrototypes();
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
    private void PopulateArrayOfNotMonsterPrototypes()
    {
        notMonsterPrototypes = new List<Card>();
        for (int i = 0; i < allCardPrototypes.Length; i++)
        {
            if (allCardPrototypes[i].Classe != Classes.MONSTER)
            {
                notMonsterPrototypes.Add(allCardPrototypes[i]);
            }
        }
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

        return ReplaceTheRandomCards( enemyDeckBuilder.GetDeck() );
    }
    #region Public Prepare XXXX Deck For The Enemy
    public static void PrepareTrainingDummyDeckForTheEnemy()
    {
        enemyDeckBuilder = new TrainingDummyDeckBuilder( Mathf.FloorToInt(DefaultDeckSize * 0.75f) );
    }
    public static void PrepareModifiedSizeRandomDeckForTheEnemy(float sizeMultiplier)
    {
        enemyDeckBuilder = new RandomDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier));
    }
    public static void PrepareModifiedSizeRandomDeckWithoutMonstersForTheEnemy(float sizeMultiplier)
    {
        enemyDeckBuilder = new NoMonstersDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier));
    }
    public static void PrepareHalfClassDeckForTheEnemy(float sizeMultiplier, Classes classe)
    {
        enemyDeckBuilder = new HalfRandomDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier), classe);
    }
    public static void PrepareFullClassDeckForTheEnemy(float sizeMultiplier, Classes classe)
    {
        enemyDeckBuilder = new FullClassDeckBuilder(Mathf.CeilToInt(DefaultDeckSize * sizeMultiplier), classe);
    }
    #endregion

    #region Get Player's Deck
    public static Card[] GetPreparedCardsForThePlayerOrGetRandomDeck()
    {
        Card[] playerDeck = GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses();

        DeckBuilder.Shuffle(ref playerDeck);

        playerDeck = ReplaceTheRandomCards(playerDeck);

        playerDeck = ReplaceMonsters(playerDeck);

        return ApplyPlayerBonuses(playerDeck);
    }
    private static Card[] GetPlayerPreparedDeckWithTheRandomCardsAndWithoutBonuses()
    {
        Card[] playerDeck;

        if (trainingDeckForThePlayer != null)
        {
            playerDeck = trainingDeckForThePlayer.GetDeck();
            trainingDeckForThePlayer = null;
        }
        else if (saveFacade.IsDeckLoaded())
        {
            DeckSerializable deckSerializable = saveFacade.GetLoadedDeck();
            PrepareLoadedDeckForThePlayer(deckSerializable.GetCardsIndexes());
            playerDeck = playerDeckBuilder.GetDeck();
        }
        else
        {
            PrepareRandomDeckForThePlayerAndSaveItInStorage();
            playerDeck = playerDeckBuilder.GetDeck();
        }

        return playerDeck;
    }
    private static Card[] ReplaceTheRandomCards(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i].IsAnotherInstanceOf(deckPrototypeFactory.theRandomCard))
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneOfCardFromPrototypesRandomly();
            }
        }

        return playerDeck;
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
    private static Card GetCloneOfCardFromPrototypesRandomly()
    {
        Card[] prototypes = deckPrototypeFactory.allCardPrototypes;
        int randomIndex = UnityEngine.Random.Range(0, prototypes.Length);
        return prototypes[randomIndex].GetClone();
    }
    private static Card GetCloneFromNotMonsterPrototypesRandomly()
    {
        List<Card> prototypes = deckPrototypeFactory.notMonsterPrototypes;
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

    #region Prepare Player's Deck
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

        int[] cardIndexes = GetArrayFilledWithTheRandomCardIndex(adjustedSize);
        SaveIndexesInStorage(cardIndexes);
    }
    public static void PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(Card[] cards)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cards);

        int[] cardIndexes = ((ManualDeckBuider)playerDeckBuilder).GetIndexOfEachCardPrototype();
        SaveIndexesInStorage(cardIndexes);
    }
    public static void PrepareLoadedDeckForThePlayer(int[] cardIndexes)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cardIndexes);
    }
    public static void PrepareTrainingDeckForThePlayer()
    {
        // TODO: remove duplicated code
        if (playerDeckBuilder == null)
        {
            Card[] theCards = new Card[4];

            for (int i = 0; i < theCards.Length; i++)
            {
                // Get some cards without skill, and others random.
                if (i % 2 == 0)
                {
                    theCards[i] = ClassInfo.GetCardsOfClass(Classes.REGULAR)[0].GetClone();
                }
                else
                {
                    theCards[i] = GetCloneOfTheRandomCard();
                }
            }

            trainingDeckForThePlayer = ManualDeckBuider.Create(theCards);
        }
        else
        {
            Card[] cards = playerDeckBuilder.GetDeck();
            Card[] theCards = new Card[4];

            for (int i = 0; i < theCards.Length; i++)
            {
                // Get some cards without skill, and others random.
                if (i % 2 == 0)
                {
                    theCards[i] = i < cards.Length ? cards[i] : ClassInfo.GetCardsOfClass(Classes.REGULAR)[0].GetClone();
                }
                else
                {
                    theCards[i] = i < cards.Length ? cards[i] : GetCloneOfTheRandomCard();
                }
            }

            trainingDeckForThePlayer = ManualDeckBuider.Create(theCards);
        }
    }
    #endregion

    public static int[] GetArrayFilledWithTheRandomCardIndex()
    {
        return GetArrayFilledWithTheRandomCardIndex(DefaultDeckSize);
    }
    private static int[] GetArrayFilledWithTheRandomCardIndex(int adjustedSize)
    {
        int[] cardIndexes = new int[adjustedSize];
        for (int i = 0; i < adjustedSize; i++)
        {
            cardIndexes[i] = ManualDeckBuider.INDEX_OF_THE_RANDOM_CARD;
        }
        return cardIndexes;
    }

    private static void SaveIndexesInStorage(int[] cardIndexes)
    {
        DeckSerializable deckSerializable = new DeckSerializable(cardIndexes);
        saveFacade.PrepareDeckForSaving(deckSerializable);
    }

    public abstract class DeckBuilder
    {
        protected int size;
        protected Card[] deck;
        protected readonly Card[] allCardPrototypes;
        protected readonly List<Card> notMonsterPrototypes;

        public DeckBuilder(int size)
        {
            this.size = size;
            allCardPrototypes = deckPrototypeFactory.allCardPrototypes;
            notMonsterPrototypes = deckPrototypeFactory.notMonsterPrototypes;
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

        protected Card GetCloneOfTrainingDummyCard()
        {
            return deckPrototypeFactory.trainingDummyCard.GetClone();
        }
    }
}

