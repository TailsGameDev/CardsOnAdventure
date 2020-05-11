
using UnityEngine;

public class UIMainMenu : UIPauseMenu
{
    // Inherits OnSettingsBtnClicked, OnRulesBtnClicked, OpenScene

    private void Awake()
    {
        MapsCache.SpotToClearIfPlayerWins = null;
    }

    public void OnPlayBtnClicked()
    {
        OpenContinueOrNewGamePopUp();
        /*
        customPopUpOpener.Open(
            title: "Play",
            warningMessage: "Would you like to play the Adventure or the Draft mode?",
            confirmBtnMessage: "Adventure Mode",
            cancelBtnMessage: "Draft Mode",
            onConfirm: OpenContinueOrNewGamePopUp,
            onCancel: NewGame
        );
        */
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
#if UNITY_WEBGL
            warningMessage = "Will you try to continue the adventure, or generate a randomized new map? (Save System"+
                " works properly just in downloadable builds)";
#else
            //warningMessage = "Are you going to continue your previous adventure, or start a new game (with randomized master spots)?";
            warningMessage = "Will you try to continue the adventure, or generate a randomized new map? (Save System" +
    " works properly only in downloadable builds)";
#endif
            continueBtnText = "Continue";
            cancelBtnMessage = "New Map";
        }
        else
        {
            //warningMessage = "Some choices in the game are mere role play. But You find out which ones";
            warningMessage = "Some pop-up buttons in the game don’t actually modify the gameplay. It’s just “role play”. " +
                                 "Like these two below.";
            continueBtnText = "Sounds Nice";
            cancelBtnMessage = "Pfff boring...";
        }

        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        customPopUpOpener.Open(
            title: "Play",
            warningMessage,
            confirmBtnMessage: continueBtnText,
            cancelBtnMessage,
            onConfirm: Continue,
            onCancel: NewGame
        );
    }

    private void Continue()
    {
        Map.StartOfMatch = false;
        sceneOpener.OpenMapScene();
    }

    private void NewGame()
    {
        Map.StartOfMatch = true;
        DeckPrototypeFactory.PrepareRandomDeckForThePlayerAndSaveItInStorage();
        sceneOpener.OpenMapScene();
    }

    public void OnTutorialMainMenuBtnClicked()
    {
        sceneOpener.OpenTutorialMainMenuScene();
    }

    public void OnCreditsBtnClicked()
    {
        sceneOpener.OpenScene("Credits");
    }

    public void OnQuitGameBtnClicked()
    {
        string warningMessage = " We dedicated so much to build this game";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, Application.Quit);
    }
}
