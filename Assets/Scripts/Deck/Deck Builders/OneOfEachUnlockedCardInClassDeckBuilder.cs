using UnityEngine;

public class OneOfEachUnlockedCardInClassDeckBuilder : DeckBuilderSuperclass
{
    private Card[] unlockedCardPrototypes;
    private OneOfEachUnlockedCardInClassDeckBuilder(int size) : base(size)
    {
    }

    public static OneOfEachUnlockedCardInClassDeckBuilder Create(Classes classe)
    {
        Card[] allCardsOfClass = ClassInfo.GetCardsOfClass(classe);
        Card[] unlockedPrototypes = CardsCollection.GetUnlockedCardsFrom( allCardsOfClass );
        OneOfEachUnlockedCardInClassDeckBuilder builder = new OneOfEachUnlockedCardInClassDeckBuilder(unlockedPrototypes.Length);
        builder.unlockedCardPrototypes = unlockedPrototypes;
        return builder;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < unlockedCardPrototypes.Length; i++)
        {
            deck[i] = unlockedCardPrototypes[i].GetClone();
        }

        return deck;
    }
    public override int GetInitialHP()
    {
        return size;
    }
}
