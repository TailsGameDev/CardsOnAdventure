public class BeginTurn : TurnBattleState
{
    public BeginTurn(Battlefield battlefield, Deck deck, Hand hand)
    {
        this.battlefield = battlefield;
        this.deck = deck;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {

    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (IveLost())
        {
            nextState = currentBattleStatesFactory.CreateEndGameState(winnerFactory: GetTheOtherFactory());
        }
        else
        {
            nextState = currentBattleStatesFactory.CreateDrawCardState();
        }

        return nextState;
    }
}
