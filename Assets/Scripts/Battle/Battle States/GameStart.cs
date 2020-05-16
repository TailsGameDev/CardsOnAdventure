using UnityEngine;

public class GameStart : BattleState
{
    public GameStart(BattleStatesFactory firstToPlayStatesFactory, BattleStatesFactory playerStatesFactory, BattleStatesFactory enemyStatesFactory, AudioRequisitor audioRequisitor)
    {
        currentBattleStatesFactory = firstToPlayStatesFactory;
        playerBattleStatesFactory = playerStatesFactory;
        enemyBattleStatesFactory = enemyStatesFactory;
        if (bgm != null)
        {
            audioRequisitor.RequestBGM(bgm);
            bgm = null;
        }

        // Just in case
        TimeFacade.RestoreTimeInNextFrameIfAllPopUpsAreClosed();
    }

    public override void ExecuteAction()
    {
        Camera.main.backgroundColor = backgroundColor;
        Card.ResetDeathCount();
    }

    public override BattleState GetNextState()
    {
        return currentBattleStatesFactory.CreateDrawCardState();
    }
}
