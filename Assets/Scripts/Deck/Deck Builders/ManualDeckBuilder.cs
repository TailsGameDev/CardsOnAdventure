public class ManualDeckBuider : DeckPrototypeFactory.DeckBuilder
{
    private int[] indexOfEachCardPrototype;
    public static readonly int INDEX_OF_RANDOM_CARD = -1;

    #region Initialization
    private ManualDeckBuider(int size) : base(size)
    {
        indexOfEachCardPrototype = new int[size];
    }

    public static ManualDeckBuider Create(Card[] cardsToBeOnDeck)
    {
        ManualDeckBuider builder = new ManualDeckBuider(cardsToBeOnDeck.Length);

        builder.FindAndCacheThePrototypeIndexForEachCard(cardsToBeOnDeck);

        return builder;
    }

    public static ManualDeckBuider Create(int[] indexOfEachCardPrototype)
    {
        ManualDeckBuider builder = new ManualDeckBuider(indexOfEachCardPrototype.Length);
        builder.indexOfEachCardPrototype = indexOfEachCardPrototype;
        return builder;
    }
    
    private void FindAndCacheThePrototypeIndexForEachCard(Card[] cardsToBeOnDeck)
    {
        for (int i = 0; i < size; i++)
        {
            // Find
            Card cardOfDeck = cardsToBeOnDeck[i];
            int prototypeIndex = FindIndexOnPrototypesArray(cardOfDeck);
            // Cache
            indexOfEachCardPrototype[i] = prototypeIndex;
        }
    }

    private int FindIndexOnPrototypesArray(Card card)
    {
        int prototypeIndex = INDEX_OF_RANDOM_CARD;
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
            if (prototypeIndex != INDEX_OF_RANDOM_CARD)
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
}