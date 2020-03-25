using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : PopUpOpener
{
    [SerializeField]
    private GameObject pauseMenuGameObject = null;

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
        string warningMessage = "All progress will be lost.";
        popUpOpener.OpenConfirmationRequestPopUp(warningMessage, OpenMainMenuScene);
    }

    public void OnGoImmediateToMainMenuBtnClicked()
    {
        OpenMainMenuScene();
    }

    private void OpenMainMenuScene()
    {
        OpenScene("Main Menu");
        if (pauseMenuGameObject != null)
        {
            pauseMenuGameObject.SetActive(false);
        }
        popUpOpener.CloseMapPopUp();
    }

    public void OpenScene(string name)
    {
        popUpOpener.OpenLoadingPopUp();
        SceneManager.LoadSceneAsync(name);
    }
}
