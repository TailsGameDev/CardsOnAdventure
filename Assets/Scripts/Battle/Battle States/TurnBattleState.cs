public abstract class TurnBattleState : BattleState
{
    protected Battlefield battlefield;
    protected Hand hand;
    protected Duelist duelist;

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

    protected bool HasPlayerLost()
    {
        return duelist.HealthPoints <= 0;
    }
}
