using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : UIPauseMenu
{
    // Inherits OnSettingsBtnClicked, OnRulesBtnClicked, OpenScene

    [SerializeField]
    private MapsCacheGetter mapsCache = null;

    [SerializeField]
    private ClassesPersistence classesPersistence = null;

    private void Awake()
    {
        MapsCache.SpotToClearIfPlayerWins = null;
    }

    public void OnPlayBtnClicked()
    {
        // If there is no save, there should'nt be a difference between press "continue" or "new game" button.
        // So I tried to make som fun out of it, and just lead the player to click one of the buttons.
        
        string continueBtnText;
        string warningMessage;
        string cancelBtnMessage;

        if (mapsCache.DoesSaveExist())
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
        if (classesPersistence.DoSaveExists())
        {
            ClassesSerializable classesSerializable = classesPersistence.LoadClasses();
            ClassInfo.LoadBonuses(classesSerializable);
        }
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
