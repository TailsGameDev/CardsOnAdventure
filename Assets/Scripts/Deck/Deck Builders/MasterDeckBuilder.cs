using System.Collections.Generic;

public class MasterDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private Classes classe;
    public MasterDeckBuilder(int size, Classes classe) : base(size)
    {
        this.classe = classe;
    }

    public override Card[] GetDeck()
    {
        CreateEmptyDeckWithProperSize();

        Card[] cardsOfClass = ClassInfo.GetCardsOfClass(classe);

        // Cards of class
        for (int i = 0; i < deck.Length/2; i++)
        {
            deck[i] = cardsOfClass[i % cardsOfClass.Length].GetClone();
        }

        // Random ones
        for (int i = deck.Length / 2; i < deck.Length; i++)
        {
            deck[i] = notMonsterPrototypes[i % notMonsterPrototypes.Count].GetClone();
        }

        // Make class stronger
        for (int i = 0; i < deck.Length; i++)
        {
            if (deck[i].Classe == classe)
            {
                if ( i%2 == 0)
                {
                    deck[i].AttackPower++;
                }
                else
                {
                    deck[i].InconditionalHealing( (i + 1) % 2 );
                    deck[i].SetInitialAndLimitVitality();
                }
            }
        }

        return deck;
    }
}
