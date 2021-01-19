using System.Collections.Generic;
using UnityEngine;

public class DeckPrototypeFactory : MonoBehaviour
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    private static DeckBuilder enemyDeckBuilder;
    private static DeckBuilder playerDeckBuilder;

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

    private void Start()
    {
        PopulateCardArraysIfNeeded();
    }
    private void PopulateCardArraysIfNeeded()
    {
        if ( ! CardArraysArePopulated())
        {
            PopulateArrayOfAllCardPrototypes();
            PopulateArrayOfNotMonsterPrototypes();
        }
    }
    private bool CardArraysArePopulated()
    {
        return deckPrototypeFactory.allCardPrototypes != null;
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

    public static void AddCardsOfClassToCollection(Classes classe)
    {
        for (int c = 0; c < deckPrototypeFactory.allCardPrototypes.Length; c++)
        {
            Card card = deckPrototypeFactory.allCardPrototypes[c];
            if (card.Classe == classe)
            {
                SumInPlayerCardsCollection(card, 1);
            }
        }
    }

    public static void SumInPlayerCardsCollection(Card card, int amountToAdd)
    {
        int c = GetIndexInAllCardPrototypesArray(card);

        int[] cardsCollectionAmounts = GetCardsCollectionAmounts();
        // c+1 because the random card index is 0.
        cardsCollectionAmounts[c] += amountToAdd;
        saveFacade.PrepareCardsCollectionForSaving(new DeckSerializable(cardsCollectionAmounts));
    }
    private static int GetIndexInAllCardPrototypesArray(Card card)
    {
        int c = -1;
        for (int i = 0; i < deckPrototypeFactory.allCardPrototypes.Length; i++)
        {
            if (card.IsAnotherInstanceOf(deckPrototypeFactory.allCardPrototypes[i]))
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
            cardAmounts = saveFacade.GetLoadedCardsCollection().cardsIndexes;
        }
        else
        {
            cardAmounts = new int[deckPrototypeFactory.allCardPrototypes.Length];
            for (int i = 0; i < cardAmounts.Length; i++)
            {
                // For now, let's consider the collection starts with one card of each type.
                // Maybe this change later when unlock new types of card, maybe not!
                cardAmounts[i] = 1;
            }
        }
        return cardAmounts;
    }

    public static Card[] GetClonesOfCardsOnPlayerCollection()
    {
        return OneOfEachButNoMonstersDeckBuilder.Create().GetDeck();
    }

    public static int GetAmountOfCardPrototypes()
    {
        deckPrototypeFactory.PopulateCardArraysIfNeeded();
        return deckPrototypeFactory.allCardPrototypes.Length;
    }

    public static Card[] GetPreparedCardsForTheEnemy()
    {
        if (enemyDeckBuilder == null)
        {
            enemyDeckBuilder = new RandomDeckBuilder( DefaultDeckSize );
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
    private static Card[] ConcatenateDecks(Card[] deck1, Card[] deck2)
    {
        Card[] megadeck = new Card[deck1.Length + deck2.Length];

        deck1.CopyTo(megadeck, 0);
        deck2.CopyTo(megadeck, deck1.Length);
        
        return megadeck;
    }
    #region Public Prepare XXXX Deck For The Enemy
    public static void PrepareEditorMadeDeckForTheEnemy(string deckName)
    {
        enemyDeckBuilder = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder(deckName);
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

        if (saveFacade.IsDeckLoaded())
        {
            DeckSerializable deckSerializable = saveFacade.GetLoadedDeck();
            PrepareLoadedDeckForThePlayer(deckSerializable.GetCardsIndexes());
            playerDeck = playerDeckBuilder.GetDeck();
        }
        else
        {
            PrepareFirstDeckIfNeededForThePlayerAndSaveItInStorage();
            playerDeck = playerDeckBuilder.GetDeck();
        }

        return playerDeck;
    }
    public static Card[] ReplaceTheRandomCards(Card[] playerDeck)
    {
        for (int i = 0; i < playerDeck.Length; i++)
        {
            if (playerDeck[i].IsAnotherInstanceOf(deckPrototypeFactory.theRandomCard))
            {
                Destroy(playerDeck[i].gameObject);
                playerDeck[i] = GetCloneOfCardFromPrototypesRandomlyButNotTheTrainingDummy();
            }
        }

        return playerDeck;
    }
    public static Card[] ReplaceMonsters(Card[] playerDeck)
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
    private static Card GetCloneOfCardFromPrototypesRandomlyButNotTheTrainingDummy()
    {
        Card[] prototypes = deckPrototypeFactory.allCardPrototypes;
        int randomIndex = Random.Range(0, prototypes.Length);
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
    public static void PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(Card[] cards)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cards);

        int[] cardIndexes = ((ManualDeckBuider)playerDeckBuilder).GetIndexOfEachCardPrototype();
        SaveDeckIndexesInStorage(cardIndexes);
    }
    public static void PrepareLoadedDeckForThePlayer(int[] cardIndexes)
    {
        playerDeckBuilder = ManualDeckBuider.Create(cardIndexes);
    }
    public static void PrepareFirstDeckIfNeededForThePlayerAndSaveItInStorage(bool forceToPrepare = false)
    {
        if (playerDeckBuilder == null || forceToPrepare)
        {
            playerDeckBuilder = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder("PlayerDeck");
            int[] cardIndexes = ((EditorMadeDeckBuilder)playerDeckBuilder).GetIndexOfEachCardPrototype();
            SaveDeckIndexesInStorage(cardIndexes);
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

    private static void SaveDeckIndexesInStorage(int[] cardIndexes)
    {
        DeckSerializable deckSerializable = new DeckSerializable(cardIndexes);
        saveFacade.PrepareDeckForSaving(deckSerializable);
    }

    public static int FindIndexOnPrototypesArray(Card card)
    {
        // -1 can be interpreted as the Random Card
        int prototypeIndex = -1;
        Card[] allCardPrototypes = deckPrototypeFactory.allCardPrototypes;
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

    public static void UpdatePrototypesLevel()
    {
        for (int p = 0; p < deckPrototypeFactory.allCardPrototypes.Length; p++)
        {
            deckPrototypeFactory.allCardPrototypes[p].RefreshStats();
        }
    }

    public abstract class DeckBuilder
    {
        protected int size;
        protected Card[] deck;
        protected readonly Card[] allCardPrototypes;
        protected readonly List<Card> notMonsterPrototypes;

        public static readonly int INDEX_OF_THE_RANDOM_CARD = -1;

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

        protected int[] FindThePrototypeIndexForEachCard(Card[] cardsToBeOnDeck)
        {
            int[] indexOfEachCardPrototype = new int[size];
            for (int i = 0; i < size; i++)
            {
                // Find
                Card cardOfDeck = cardsToBeOnDeck[i];
                int prototypeIndex = FindIndexOnPrototypesArray(cardOfDeck);
                // Cache
                indexOfEachCardPrototype[i] = prototypeIndex;
            }
            return indexOfEachCardPrototype;
        }
    }
}

