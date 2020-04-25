public class DrawCard : BattleState
{
    private Deck deck;
    private Battlefield battlefield;
    private Hand hand;

    public DrawCard(Deck deck, Battlefield battlefield, Hand hand)
    {
        this.deck = deck;
        this.battlefield = battlefield;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {
        if (deck.ContainCards() && !hand.IsFull())
        {
            Card firstOfDeck = deck.DrawCard();
            hand.AddCard(firstOfDeck);
        }
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (hand.IsFull())
        {
            if (battlefield.IsFull())
            {
                nextState = currentBattleStatesFactory.CreateRepositionState();
            }
            else
            {
                nextState = currentBattleStatesFactory.CreatePlaceCardState();
            }
        }
        else
        {
            if (deck.IsEmpty())
            {
                nextState = currentBattleStatesFactory.CreatePlaceCardState();
            }
            else
            {
                // Draw more cards!
                nextState = this;
            }
        }

        return nextState;
    }
}
