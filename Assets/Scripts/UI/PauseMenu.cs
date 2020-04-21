using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : PopUpOpener
{
    [SerializeField]
    protected SceneOpener sceneOpener = null;

    public void OnRulesBtnClicked()
    {
        popUpOpener.OpenRulesPopUp();
    }

    public void OnSettingsBtnClicked()
    {
        popUpOpener.OpenSettingsPopUp();
    }

    public void OnGoToMainMenuBtnClicked()
    {
        string warningMessage = "All unsaved progress will be lost.";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, sceneOpener.OpenMainMenuScene);
    }

    public void OnGoImmediateToMainMenuBtnClicked()
    {
        sceneOpener.OpenMainMenuScene();
    }


}
