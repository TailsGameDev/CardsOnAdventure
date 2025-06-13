using UnityEngine;

public class ManualDeckBuider : DeckBuilderSuperclass
{
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

    public static ManualDeckBuider Create(int[] indexOfEachCardPrototype)
    {
        ManualDeckBuider builder = new ManualDeckBuider(indexOfEachCardPrototype.Length);
        builder.indexOfEachCardPrototype = indexOfEachCardPrototype;
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
}