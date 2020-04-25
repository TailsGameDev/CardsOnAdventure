using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : BattleState
{
    private BattleStatesFactory winnerFactory;
    private ThePopUpOpenerInstance popUpOpener;
    private CustomPopUp customPopUpOpener;
    private PreMadeSoundRequest victoryBGMRequest;
    private PreMadeSoundRequest defeatBGMRequest;
    private PreMadeSoundRequest stopAllSFXRequest;

    private float timer = 0;

    private bool quit = false;

    public EndGame(BattleStatesFactory winnerFactory, ThePopUpOpenerInstance popUpOpener, CustomPopUp customPopUpOpener, PreMadeSoundRequest victoryBGMRequest, PreMadeSoundRequest defeatBGMRequest, PreMadeSoundRequest stopAllSFXRequest)
    {
        this.winnerFactory = winnerFactory;
        this.popUpOpener = popUpOpener;
        this.customPopUpOpener = customPopUpOpener;
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
                                title: "You've beaten a Master!",
                                warningMessage: "All your cards of class " + masterClass + " level up!\nPlease, choose what to improve on them!",
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
                            confirmBtnMessage: "Go to main menu",
                            cancelBtnMessage: "Sit and cry",
                            QuitBattleLoadMainMenu,
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
        popUpOpener.CloseAllPopUpsExceptLoading();
        popUpOpener.OpenMapScene();
    }

    private void ImproveAttackPowerThenSeeMap()
    {
        ClassInfo.GiveAttackPowerBonusToClassAndSaveInDeviceStorage(masterClass);
        QuitBattleAndGoToMap();
    }

    private void ImproveVitalityThenSeeMap()
    {
        ClassInfo.GiveVitalityBonusToClassAndSaveInDeviceStorage(masterClass);
        QuitBattleAndGoToMap();
    }

    private void QuitBattleLoadMainMenu()
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        popUpOpener.CloseAllPopUpsExceptLoading();
        stopAllSFXRequest.RequestPlaying();
        quit = true;
        SceneManager.LoadScene("Main Menu");
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
