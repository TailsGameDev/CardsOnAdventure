
using UnityEngine;

public class UIMainMenu : UIPauseMenu
{
    // Inherits OnSettingsBtnClicked, OnRulesBtnClicked, OpenScene

    // private SaveMediator saveMediator = new SaveMediator();

    private void Awake()
    {
        MapsCache.SpotToClearIfPlayerWins = null;
    }

    public void OnPlayBtnClicked()
    {
        ClassInfo.ResetBonusesToAllClasses();

        OpenContinueOrNewGamePopUp();
        /*
        customPopUpOpener.Open(
            title: "Play!",
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
            continueBtnText = "Continue";
            warningMessage = "Are you going to continue your previous adventure, or start a new game?";
            cancelBtnMessage = "New Game";
        }
        else
        {
            warningMessage = "Please press one of these two buttons. There is no time to explain!";
            continueBtnText = "Yellow";
            cancelBtnMessage = "Red";
        }

        popUpOpener.CloseAllPopUpsExceptLoading();
        customPopUpOpener.Open(
            title: "Play!",
            warningMessage,
            confirmBtnMessage: continueBtnText,
            cancelBtnMessage,
            onConfirm: Continue,
            onCancel: NewGame
        );
    }

    private void Continue()
    {
        UIMap.StartOfMatch = false;
        sceneOpener.OpenMapScene();
    }

    private void NewGame()
    {
        UIMap.StartOfMatch = true;
        sceneOpener.OpenMapScene();
    }

    public void OnCreditsBtnClicked()
    {
        sceneOpener.OpenScene("Credits");
    }

    public void OnQuitGameBtnClicked()
    {
        string warningMessage = " We dedicated so mutch to build this game! ";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, Application.Quit);
    }
}
