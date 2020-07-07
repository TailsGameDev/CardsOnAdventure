public class OneOfEachAndARandomCardButNoMonstersDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private OneOfEachAndARandomCardButNoMonstersDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachAndARandomCardButNoMonstersDeckBuilder Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachAndARandomCardButNoMonstersDeckBuilder me = 
            new OneOfEachAndARandomCardButNoMonstersDeckBuilder(SENTINEL_NUMBER);
        me.size = me.notMonsterPrototypes.Count + 1;
        return me;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        deck[0] = GetCloneOfTheRandomCard();

        for (int i = 1; i < deck.Length; i++)
        {
            deck[i] = notMonsterPrototypes[i - 1].GetClone();
        }

        return deck;
    }
}

