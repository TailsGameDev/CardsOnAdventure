using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : PopUpOpener
{
    [SerializeField]
    protected SceneOpener sceneOpener = null;

    public void OnWhatTheTeachingSpiritIsBtnClicked()
    {
        popUpOpener.OpenWhatTheTeachingSpiritIsPopUp();
    }

    public void OnBattleRulesBtnClicked()
    {
        popUpOpener.OpenBattleRulesPopUp();
    }

    public void OnSettingsBtnClicked()
    {
        popUpOpener.OpenSettingsPopUp();
    }

    public void OnQuitToMainMenuBtnClicked()
    {
        string warningMessage = "All unsaved progress will be lost.";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, sceneOpener.OpenMainMenuScene);
    }

    public void OnGoImmediateToMainMenuBtnClicked()
    {
        sceneOpener.OpenMainMenuScene();
    }


}
