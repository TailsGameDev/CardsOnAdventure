using System.Collections.Generic;
using UnityEngine;

public class DeckPrototypeFactory : CardPrototypesAccessor
{
    private static DeckPrototypeFactory deckPrototypeFactory;

    [SerializeField]
    private int defaultDeckSize = -1;

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
        PopulateCardDataStructuresIfNeeded();
    }
    private void PopulateCardDataStructuresIfNeeded()
    {
        if ( ! CardArraysArePopulated())
        {
            CardPrototypesAccessor.theRandomCardPrototype = theRandomCard;
            PopulateArrayOfAllCardPrototypes();
            PopulateArrayOfNotMonsterPrototypes();
        }
    }
    private bool CardArraysArePopulated()
    {
        return allCardPrototypes != null;
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

    #region Collection
    public static void AddCardsOfClassToCollection(Classes classe)
    {
        for (int c = 0; c < allCardPrototypes.Length; c++)
        {
            Card card = allCardPrototypes[c];
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
        for (int i = 0; i < allCardPrototypes.Length; i++)
        {
            if (card.IsAnotherInstanceOf(allCardPrototypes[i]))
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
            cardAmounts = new int[allCardPrototypes.Length];
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
    #endregion

    public static int GetAmountOfCardPrototypes()
    {
        deckPrototypeFactory.PopulateCardDataStructuresIfNeeded();
        return allCardPrototypes.Length;
    }   

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

    public static void UpdatePrototypesLevel()
    {
        for (int p = 0; p < allCardPrototypes.Length; p++)
        {
            allCardPrototypes[p].RefreshStats();
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
            this.allCardPrototypes = CardPrototypesAccessor.allCardPrototypes;
            this.notMonsterPrototypes = CardPrototypesAccessor.notMonsterPrototypes;
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

