using UnityEngine;
public class RandomDeckBuilder : DeckBuilderSuperclass
{
    public RandomDeckBuilder(int size) : base(size)
    {
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = GetCloneOfTheRandomCard();
        }

        return deck;
    }

    public override int GetInitialHP()
    {
        return size;
    }
}
