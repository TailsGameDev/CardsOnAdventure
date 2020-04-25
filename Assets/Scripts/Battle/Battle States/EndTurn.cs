public class EndTurn : TurnBattleState
{
    public EndTurn(Battlefield battlefield, Deck deck, Hand hand)
    {
        this.battlefield = battlefield;
        this.deck = deck;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {
        battlefield.RemoveFreezingStateFromAllCards();
        battlefield.RemoveObfuscateFromAllCards();

        currentBattleStatesFactory = GetTheOtherFactory();
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (IveLost())
        {
            nextState = currentBattleStatesFactory.CreateEndGameState(winnerFactory: currentBattleStatesFactory);
        }
        else if (Card.GetDeathCount() <= 0)
        {
            nextState = currentBattleStatesFactory.CreateIsGameTiedState();
        }
        else
        {
            nextState = currentBattleStatesFactory.CreateBeginTurnState();
        }
        return nextState;
    }
}
