using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : BattleState
{
    private BattleStatesFactory winnerFactory;
    private ThePopUpOpenerInstance popUpOpener;
    private CustomPopUp customPopUpOpener;
    private SceneOpener sceneOpener;
    private PreMadeSoundRequest victoryBGMRequest;
    private PreMadeSoundRequest defeatBGMRequest;
    private PreMadeSoundRequest stopAllSFXRequest;

    private float timer = 0;

    private bool quit = false;

    public EndGame(BattleStatesFactory winnerFactory,
                    ThePopUpOpenerInstance popUpOpener,
                    CustomPopUp customPopUpOpener,
                    SceneOpener sceneOpener,
                    PreMadeSoundRequest victoryBGMRequest,
                    PreMadeSoundRequest defeatBGMRequest,
                    PreMadeSoundRequest stopAllSFXRequest)
    {
        this.winnerFactory = winnerFactory;
        this.popUpOpener = popUpOpener;
        this.customPopUpOpener = customPopUpOpener;
        this.sceneOpener = sceneOpener;
        this.victoryBGMRequest = victoryBGMRequest;
        this.defeatBGMRequest = defeatBGMRequest;
        this.stopAllSFXRequest = stopAllSFXRequest;
    }
    public override void ExecuteAction()
    {
        if (!quit)
        {
            if (timer < 1.5f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (popUpOpener.AllPopUpsAreClosed())
                {
                    timer = 0;

                    if (winnerFactory == playerBattleStatesFactory)
                    {
                        if (IsMasterBattle())
                        {
                            customPopUpOpener.Open(
                                title: "You beat a Master!",
                                warningMessage: "<color=#FFFFFF>And then you 'borrowed' some of their equipment!</color>" +
                                   Formater.Paint(" ALL YOUR " + masterClass + " CARDS WILL BE BUFFED. PLEASE CHOOSE:", backgroundColor),
                                confirmBtnMessage: "+1 Vitality",
                                cancelBtnMessage: "+1 Attack Power",
                                onConfirm: ImproveVitalityThenSeeMap,
                                onCancel: ImproveAttackPowerThenSeeMap,
                                victoryBGMRequest
                            );
                        }
                        else
                        {
                            customPopUpOpener.Open(
                                title: "Congratulations!",
                                warningMessage: "You beat those guys. What are you going to do now?",
                                confirmBtnMessage: "Look the map!",
                                cancelBtnMessage: "Nothing",
                                QuitBattleAndGoToMap,
                                victoryBGMRequest
                            );
                        }
                    }
                    else
                    {
                        MapsCache.SpotToClearIfPlayerWins = null;
                        customPopUpOpener.Open(
                            title: "You've lost the battle",
                            warningMessage: "The enemy start to search you fallen card's pockets",
                            confirmBtnMessage: "Go back in time",
                            cancelBtnMessage: "Sit and cry",
                            GoBackInTime,
                            defeatBGMRequest
                        );
                    }
                }
                else
                // Some pop up is oppened
                {
                    timer = 0.0f;
                }
            }
        }

    }
    private void QuitBattleAndGoToMap()
    {
        quit = true;
        stopAllSFXRequest.RequestPlaying();
        sceneOpener.OpenMapScene();
    }
    private void ImproveAttackPowerThenSeeMap()
    {
        ClassInfo.GiveAttackPowerBonusToClass(masterClass);
        QuitBattleAndGoToMap();
    }
    private void ImproveVitalityThenSeeMap()
    {
        ClassInfo.GiveVitalityBonusToClass(masterClass);
        QuitBattleAndGoToMap();
    }
    private void GoBackInTime()
    {
        stopAllSFXRequest.RequestPlaying();
        quit = true;
        sceneOpener.OpenBattle();
    }
    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (quit)
        {
            nextState = new Quit();
        }
        else
        {
            nextState = this;
        }

        return nextState;
    }
}
