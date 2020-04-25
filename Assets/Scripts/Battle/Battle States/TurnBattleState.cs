public abstract class TurnBattleState : BattleState
{
    protected Battlefield battlefield;
    protected Deck deck;
    protected Hand hand;

    protected BattleStatesFactory GetTheOtherFactory()
    {
        BattleStatesFactory otherFactory;
        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            otherFactory = enemyBattleStatesFactory;
        }
        else
        {
            otherFactory = playerBattleStatesFactory;
        }
        return otherFactory;
    }

    protected bool IveLost()
    {
        return deck.IsEmpty() && battlefield.IsEmpty() && hand.IsEmpty();
    }
}
