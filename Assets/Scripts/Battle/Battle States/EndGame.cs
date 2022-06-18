using System.Collections.Generic;
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
                        switch (CurrentBattleInfo.rewardType)
                        {
                            case BattleReward.NONE:
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
                                break;
                            case BattleReward.CARDS_OF_CLASS:
                                {
                                    sceneCanvas.SetActive(false);
                                    customPopUpOpener.OpenDisplayingUnblockedCardsOfClass(
                                        title: "You beat a Master",
                                        warningMessage: ColorHexCodes.BeginWhite+"What about making some recruiting?"+ColorHexCodes.End +
                                           ColorHexCodes.Paint(" YOU JUST GOT ONE OF EACH " + enemyDeckClass + " CARDS TO YOUR COLLECTION. ", deckColor),
                                        confirmBtnMessage: "Awesome",
                                        onConfirm: GiveUnblockedCardsOfClassThenSeeMap,
                                        victoryBGMRequest,
                                        enemyDeckClass
                                    );
                                }
                                break;
                            case BattleReward.SPECIFIC_CARD:
                                {
                                    sceneCanvas.SetActive(false);
                                    Card[] reward = EditorMadeDeckBuilder.CreateEditorMadeDeckBuilder(rewardDeckName).GetDeck();

                                    CustomPopUp.OnBtnClicked onbtn = () => { GiveRewardDeckThenSeeMap(reward); };

                                    customPopUpOpener.OpenDisplayingRewardCards(
                                        title: "You win!",
                                        warningMessage: ColorHexCodes.BeginWhite + "You beat the card challenge." + ColorHexCodes.End +
                                           ColorHexCodes.Paint(" YOU GOT A CARD ", deckColor),
                                        confirmBtnMessage: "Awesome",
                                        onConfirm: onbtn,
                                        victoryBGMRequest,
                                        cards: reward
                                    );
                                }
                                break;
                        }
                    }
                    else
                    {
                        MapsCache.SpotToClearAndLevelUpIfPlayerWins = null;
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
    private void GiveUnblockedCardsOfClassThenSeeMap()
    {
        CardsCollection.AddOneOfEachUnblockedCardOfClassToCollection(enemyDeckClass);
        QuitBattleAndGoToMap();
    }
    private void GiveCardThenSeeMap()
    {
        CardsCollection.SumToCurrentAmount(PlayerAndEnemyDeckHolder.GetPreparedCardsForTheEnemy()[0], 1);
        QuitBattleAndGoToMap();
    }
    private void Give3CardsThenSeeMap()
    {
        CardsCollection.SumToCurrentAmount(PlayerAndEnemyDeckHolder.GetPreparedCardsForTheEnemy()[0], 3);
        QuitBattleAndGoToMap();
    }
    private void GiveRewardDeckThenSeeMap(Card[] rewardDeck)
    {
        for (int r = 0; r < rewardDeck.Length; r++)
        {
            CardsCollection.SumToCurrentAmount(rewardDeck[r], 1);
        }
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

    private class NotUsed
    {
        EndGame endGame = null;

        // This is private class NotUsed
        private void ImproveAttackPowerThenSeeMap()
        {
            ClassInfo.GiveAttackPowerBonusToClass(enemyDeckClass);
            endGame.QuitBattleAndGoToMap();
        }
        // This is private class NotUsed
        private void ImproveVitalityThenSeeMap()
        {
            ClassInfo.GiveVitalityBonusToClass(enemyDeckClass);
            endGame.QuitBattleAndGoToMap();
        }
    }
}
