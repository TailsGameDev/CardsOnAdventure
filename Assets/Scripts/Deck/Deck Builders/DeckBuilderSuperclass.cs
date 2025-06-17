using UnityEngine;

public abstract class DeckBuilderSuperclass : CardPrototypesAccessor
{
    protected int size;
    protected Card[] deck;
    protected int healthPoints;

    public static readonly int DEFAULT_DECK_SIZE = 8;

    public static readonly int INDEX_OF_THE_RANDOM_CARD = -1;

    public DeckBuilderSuperclass(int size)
    {
        this.size = size;
    }

    public abstract Card[] GetDeck();
    public abstract int GetInitialHP();

    protected void CreateEmptyDeckWithProperSize()
    {
        if (size < 0)
        {
            Debug.LogWarning("deck size is set less than zero");
            size = DEFAULT_DECK_SIZE;
        }

        deck = new Card[size];
    }

    protected Card GetCloneOfTheRandomCard()
    {
        return theRandomCardPrototype.GetClone();
    }

    // TODO: Review if needed
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
    protected int[] FindTheAmountForEachCard(Card[] cardsToBeOnDeck)
    {
        int prototypesArrayLength = allCardPrototypes.Length;
        int[] amountOfEachCard = new int[prototypesArrayLength];
        for (int i = 0; i < cardsToBeOnDeck.Length; i++)
        {
            // Find
            Card cardOfDeck = cardsToBeOnDeck[i];
            int prototypeIndex = FindIndexOnPrototypesArray(cardOfDeck);
            // Cache
            amountOfEachCard[prototypeIndex] += 1;
        }
        return amountOfEachCard;
    }
    protected int[] FindTheAmountForEachCard(int[] cardsIndexesToBeOnDeck)
    {
        int prototypesArrayLength = allCardPrototypes.Length;
        int[] amountOfEachCard = new int[prototypesArrayLength];
        for (int i = 0; i < cardsIndexesToBeOnDeck.Length; i++)
        {
            // Find
            int prototypeIndex = cardsIndexesToBeOnDeck[i];
            // Cache
            amountOfEachCard[prototypeIndex] += 1;
        }
        return amountOfEachCard;
    }

    public static int[] GetArrayFilledWithTheRandomCardIndex()
    {
        return GetArrayFilledWithTheRandomCardIndex(DEFAULT_DECK_SIZE);
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
}