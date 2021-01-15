using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeableCardsHolder : DeckCardHolder
{
    [SerializeField]
    private int amountOfExchangeableCards = 0;

    protected override int DecideAmountOfSlotsThisCardHolderShouldHave()
    {
        return amountOfExchangeableCards;
    }
    protected override Card[] PopulateCardsArray()
    {
        RandomDeckBuilder randomDeckBuilder = new RandomDeckBuilder(amountOfExchangeableCards);
        cards = randomDeckBuilder.GetDeck();
        cards = DeckPrototypeFactory.ReplaceTheRandomCards(cards);
        cards = DeckPrototypeFactory.ReplaceMonsters(cards);
        return cards;
    }
}
