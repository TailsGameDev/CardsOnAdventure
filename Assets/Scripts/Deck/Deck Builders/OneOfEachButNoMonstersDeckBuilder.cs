using UnityEngine;
public class OneOfEachButNoMonstersDeckBuilder : DeckBuilderSuperclass
{
    private OneOfEachButNoMonstersDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachButNoMonstersDeckBuilder Create()
    {
        const int SENTINEL_NUMBER = 0;
        OneOfEachButNoMonstersDeckBuilder me = 
            new OneOfEachButNoMonstersDeckBuilder(SENTINEL_NUMBER);
        me.size = notMonsterPrototypes.Length;
        return me;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = notMonsterPrototypes[i].GetClone();
        }

        return deck;
    }
    public override int GetInitialHP()
    {
        return size;
    }
}

