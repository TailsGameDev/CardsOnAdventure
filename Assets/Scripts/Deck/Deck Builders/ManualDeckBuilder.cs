using UnityEngine;

public class ManualDeckBuider : DeckBuilderSuperclass
{
    // TODO: Consider using amounts instead of indexes, review if this class can be merged with EditorMadeDeck
    private int[] indexOfEachCardPrototype;

    #region Initialization
    private ManualDeckBuider(int size) : base(size)
    {
        indexOfEachCardPrototype = new int[size];
    }

    public static ManualDeckBuider Create(Card[] cardsToBeOnDeck)
    {
        ManualDeckBuider builder = new ManualDeckBuider(cardsToBeOnDeck.Length);

        builder.indexOfEachCardPrototype = builder.FindThePrototypeIndexForEachCard(cardsToBeOnDeck);

        return builder;
    }

    public static ManualDeckBuider Create(int[] amountOfEachCardPrototype)
    {
        // Instantiate build with proper indexes array size
        int deckSize = 0;
        for (int a = 0; a < amountOfEachCardPrototype.Length; a++)
        {
            deckSize += amountOfEachCardPrototype[a];
        }
        ManualDeckBuider builder = new ManualDeckBuider(deckSize);

        // Populate indexes array
        int indexesIndex = 0;
        for (int a = 0; a < amountOfEachCardPrototype.Length; a++)
        {
            int amountOfCardsToAdd = amountOfEachCardPrototype[a];
            while (amountOfCardsToAdd > 0)
            {
                builder.indexOfEachCardPrototype[indexesIndex] = a;
                indexesIndex++;
                amountOfCardsToAdd--;
            }
        }

        return builder;
    }
    #endregion

    public int[] GetIndexOfEachCardPrototype()
    {
        return indexOfEachCardPrototype;
    }

    public override Card[] GetDeck()
    {
        deck = new Card[size];

        for (int i = 0; i < size; i++)
        {
            int prototypeIndex = indexOfEachCardPrototype[i];
            Card card;
            if (prototypeIndex != INDEX_OF_THE_RANDOM_CARD)
            {
                card = allCardPrototypes[prototypeIndex].GetClone();
            }
            else
            {
                card = GetCloneOfTheRandomCard();
            }
            deck[i] = card;
        }

        return deck;
    }
    public override int GetInitialHP()
    {
        return size;
    }

    public int[] GetAmountForEachCardPrototype()
    {
        return FindTheAmountForEachCard(indexOfEachCardPrototype);
    }
}