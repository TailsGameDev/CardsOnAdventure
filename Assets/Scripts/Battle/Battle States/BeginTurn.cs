public class BeginTurn : TurnBattleState
{
    public BeginTurn(Duelist duelist, Battlefield battlefield, Deck deck, Hand hand)
    {
        this.battlefield = battlefield;
        this.hand = hand;
        this.duelist = duelist;
    }

    public override void ExecuteAction()
    {

    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (HasPlayerLost())
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
