
using System.Collections;
using UnityEngine;

public class UIMainMenu : UIPauseMenu
{
    // Inherits OnSettingsBtnClicked, OnRulesBtnClicked, OpenScene

    [SerializeField]
    private PreMadeAudioFactory preMadeAudioFactory = null;

    CustomPopUp.OnBtnClicked ContinueAction;
    CustomPopUp.OnBtnClicked CancelAction;

    private void Awake()
    {
        MapsCache.SpotToClearIfPlayerWins = null;
    }

    public void OnPlayBtnClicked()
    {
        OpenContinueOrNewGamePopUp();
        cardBackground.ChangeSprite();
    }

    private void OpenContinueOrNewGamePopUp()
    {
        // If there is no save, there should'nt be a difference between press "continue" or "new game" button.
        // So I tried to make som fun out of it, and just lead the player to click one of the buttons.

        string continueBtnText;
        string warningMessage;
        string cancelBtnMessage;

        if (new SaveFacade().DoesAnySaveExist())
        {
            warningMessage = "Are you going to continue your previous adventure, or start a new game, with randomized master spots?";
#if UNITY_WEBGL
            warningMessage = "Will you try to continue the adventure, or generate a randomized new map? (Save System"+
                " works properly just in downloadable builds)";
#endif
            continueBtnText = "Continue";
            cancelBtnMessage = "New Map";

            ContinueAction = Continue;
            CancelAction = NewGame;
        }
        else
        {
            //warningMessage = "Some choices in the game are mere role play. But You find out which ones";
            warningMessage = "Some pop-up buttons in the game don’t actually modify the gameplay. It’s just “role play”. " +
                                 "Like these two below.";
            continueBtnText = "Cool";
            cancelBtnMessage = "Bored pffff...";

            ContinueAction = () => { StartCoroutine(CoolContinue()); };
            CancelAction = () => { StartCoroutine(BoringNewGame()); };
        }

        

        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        customPopUpOpener.Open(
            title: "Play",
            warningMessage,
            confirmBtnMessage: continueBtnText,
            cancelBtnMessage,
            onConfirm: ContinueAction,
            onCancel: CancelAction
        );
    }

    IEnumerator CoolContinue()
    {
        preMadeAudioFactory.CreateCoolAudioRequest(gameObject).RequestPlaying();
        yield return null;
        Continue();
    }
    IEnumerator BoringNewGame()
    {
        preMadeAudioFactory.CreateBoringAudioRequest(gameObject).RequestPlaying();
        yield return null;
        NewGame();
    }

    private void Continue()
    {
        Map.StartOfMatch = false;
        Map.ShouldLoadEverything = true;
        sceneOpener.OpenMapScene();
    }

    private void NewGame()
    {
        Map.StartOfMatch = true;
        CardsLevel.Clear();
        DeckGetter.PrepareFirstDeckIfNeededForThePlayerAndGetReadyForSaving(forceToPrepare: true);
        sceneOpener.OpenMapScene();
    }

    public void OnTutorialMainMenuBtnClicked()
    {
        sceneOpener.OpenTutorialMainMenuScene();
    }

    public override void OnSettingsBtnClicked()
    {
        base.OnSettingsBtnClicked();
        cardBackground.ChangeSprite();
    }

    public void OnCreditsBtnClicked()
    {
        sceneOpener.OpenScene("Credits");
    }

    public void OnQuitGameBtnClicked()
    {
        string warningMessage = " We dedicated so much to build this game";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, Application.Quit);
        cardBackground.ChangeSprite();
    }
}
