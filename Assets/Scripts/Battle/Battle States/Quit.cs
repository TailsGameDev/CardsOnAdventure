public class Quit : BattleState
{
    public override void ExecuteAction()
    {
    }

    public override BattleState GetNextState()
    {
        return this;
    }
}
