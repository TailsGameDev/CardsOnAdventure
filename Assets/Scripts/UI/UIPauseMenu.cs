using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : OpenersSuperclass
{
    public void OnBattleRulesBtnClicked()
    {
        openerOfPopUpsMadeInEditor.OpenBattleRulesPopUp();
    }

    public void OnSettingsBtnClicked()
    {
        openerOfPopUpsMadeInEditor.OpenSettingsPopUp();
    }

    public void OnQuitToMainMenuBtnClicked()
    {
        string warningMessage = "All unsaved progress will be lost.";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, sceneOpener.OpenMainMenu);
    }

    public void OnGoImmediateToMainMenuBtnClicked()
    {
        sceneOpener.OpenMainMenu();
    }
}
