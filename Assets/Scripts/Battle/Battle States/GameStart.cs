using UnityEngine;
using UnityEngine.UI;

public class GameStart : BattleState
{
    public GameStart
        (
            BattleStatesFactory firstToPlayStatesFactory, 
            BattleStatesFactory playerStatesFactory, 
            BattleStatesFactory enemyStatesFactory, 
            AudioRequisitor audioRequisitor, 
            Image battleIconImage
        )
    {
        currentBattleStatesFactory = firstToPlayStatesFactory;
        playerBattleStatesFactory = playerStatesFactory;
        enemyBattleStatesFactory = enemyStatesFactory;
        if (bgm != null)
        {
            audioRequisitor.RequestBGMAndLoop(bgm);
            bgm = null;
        }

        // Just in case
        TimeFacade.RestoreTimeInNextFrameIfAllPopUpsAreClosed();

        if (CurrentBattleInfo.BattleIcon != null)
        {
            battleIconImage.sprite = CurrentBattleInfo.BattleIcon;
        }
    }

    public override void ExecuteAction()
    {
        Camera.main.backgroundColor = deckColor;
        Card.ResetDeathCount();
    }

    public override BattleState GetNextState()
    {
        return currentBattleStatesFactory.CreateDrawCardState();
    }
}
