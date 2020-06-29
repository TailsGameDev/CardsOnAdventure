using UnityEngine;

public class BattleIncidentAction : IncidentAction
{
    [SerializeField]
    private SpotPrototype battlePrototypeSpot = null;

    public override void Execute()
    {
        battlePrototypeSpot.PrepareBattle();
        sceneOpener.OpenBattle();
    }
}
