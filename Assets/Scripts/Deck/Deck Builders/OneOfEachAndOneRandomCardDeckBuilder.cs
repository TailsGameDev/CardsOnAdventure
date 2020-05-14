public class OneOfEachAndOneRandomCardDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private OneOfEachAndOneRandomCardDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachAndOneRandomCardDeckBuilder Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachAndOneRandomCardDeckBuilder me = new OneOfEachAndOneRandomCardDeckBuilder(SENTINEL_NUMBER);
        me.size = me.allCardPrototypes.Length+1;
        return me;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        deck[0] = GetCloneOfTheRandomCard();

        for (int i = 1; i < deck.Length; i++)
        {
            deck[i] = allCardPrototypes[i-1].GetClone();
        }

        return deck;
    }
}
