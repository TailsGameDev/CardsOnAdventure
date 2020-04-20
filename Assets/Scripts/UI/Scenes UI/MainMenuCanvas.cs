using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : PauseMenu
{
    // Inherits OnSettingsBtnClicked, OnRulesBtnClicked, OpenScene

    [SerializeField]
    private MapsCacheGetter mapsCache = null;

    public void OnPlayBtnClicked()
    {
        customPopUpOpener.Open(
            title: "Play!",
            warningMessage: "Are you going to continue your previous adventure (if any), or start a new game?",
            confirmBtnMessage: "Continue",
            cancelBtnMessage: "New Game",
            onConfirm: Continue,
            onCancel: NewGame
        );
    }

    private void Continue()
    {
        // TODO: Save bonuses of classes
        mapsCache.StartOfMatch = false;
        popUpOpener.OpenMapPopUp();
    }

    private void NewGame()
    {
        ClassInfo.ResetBonusesToAllClasses();
        mapsCache.StartOfMatch = true;
        popUpOpener.OpenMapPopUp();
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
