using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : BattleState
{
    private BattleStatesFactory winnerFactory;
    private GameObject sceneCanvas;
    private ThePopUpOpenerInstance popUpOpener;
    private CustomPopUp customPopUpOpener;
    private SceneOpener sceneOpener;
    private PreMadeAudioRequest victoryBGMRequest;
    private PreMadeAudioRequest defeatBGMRequest;
    private PreMadeAudioRequest stopAllSFXRequest;
    private PreMadeAudioRequest cricketsAudioRequest;
    private PreMadeAudioRequest cryingAudioRequest;

    private float timer = 0;

    private bool quit = false;

    public EndGame(BattleStatesFactory winnerFactory,
                    GameObject sceneCanvas,
                    ThePopUpOpenerInstance popUpOpener,
                    CustomPopUp customPopUpOpener,
                    SceneOpener sceneOpener,
                    PreMadeAudioRequest victoryBGMRequest,
                    PreMadeAudioRequest defeatBGMRequest,
                    PreMadeAudioRequest stopAllSFXRequest,
                    PreMadeAudioRequest cricketsAudioRequest,
                    PreMadeAudioRequest cryingAudioRequest)
    {
        this.winnerFactory = winnerFactory;
        this.sceneCanvas = sceneCanvas;
        this.popUpOpener = popUpOpener;
        this.customPopUpOpener = customPopUpOpener;
        this.sceneOpener = sceneOpener;
        this.victoryBGMRequest = victoryBGMRequest;
        this.defeatBGMRequest = defeatBGMRequest;
        this.stopAllSFXRequest = stopAllSFXRequest;
        this.cricketsAudioRequest = cricketsAudioRequest;
        this.cryingAudioRequest = cryingAudioRequest;
    }
    public override void ExecuteAction()
    {
        if (!quit)
        {
            if (timer < 1.5f)
            {
                timer += TimeFacade.DeltaTime;
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
                            sceneCanvas.SetActive(false);
                            customPopUpOpener.OpenDisplayingCardsOfClass(
                                title: "You beat a Master",
                                warningMessage: "<color=#FFFFFF>And then you 'borrowed' some of their equipment</color>" +
                                   Formater.Paint(" ALL YOUR " + enemyDeckClass + " CARDS WILL BE BUFFED. PLEASE CHOOSE:", backgroundColor),
                                confirmBtnMessage: "+1 Vitality",
                                cancelBtnMessage: "+1 Attack Power",
                                onConfirm: ImproveVitalityThenSeeMap,
                                onCancel: ImproveAttackPowerThenSeeMap,
                                victoryBGMRequest,
                                enemyDeckClass
                            );
                        }
                        else
                        {
                            customPopUpOpener.OpenWithBGM(
                                title: "Congratulations",
                                warningMessage: "You beat those guys. What are you going to do now?",
                                confirmBtnMessage: "Look the map",
                                cancelBtnMessage: "Nothing",
                                onConfirm: QuitBattleAndGoToMap,
                                onCancel: () => { cricketsAudioRequest.RequestPlaying(); },
                                victoryBGMRequest
                            );
                        }
                    }
                    else
                    {
                        MapsCache.SpotToClearIfPlayerWins = null;
                        customPopUpOpener.OpenWithBGM(
                            title: "You've lost the battle",
                            warningMessage: "The enemy start to search you fallen card's pockets",
                            confirmBtnMessage: "Go back in time",
                            cancelBtnMessage: "Sit and cry",
                            GoBackInTime,
                            () => { cryingAudioRequest.RequestPlaying(); },
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
        ClassInfo.GiveAttackPowerBonusToClass(enemyDeckClass);
        QuitBattleAndGoToMap();
    }
    private void ImproveVitalityThenSeeMap()
    {
        ClassInfo.GiveVitalityBonusToClass(enemyDeckClass);
        QuitBattleAndGoToMap();
    }
    private void GoBackInTime()
    {
        stopAllSFXRequest.RequestPlaying();
        quit = true;
        sceneOpener.OpenMapScene();
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
