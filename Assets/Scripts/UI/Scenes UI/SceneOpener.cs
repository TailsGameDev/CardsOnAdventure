using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpener : PopUpOpener
{
    public void OpenMapScene()
    {
        popUpOpener.OpenMapScene();
    }

    // Start is called before the first frame update
    public void OpenMainMenuScene()
    {
        OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    public void OpenDeckBuildingScene()
    {
        OpenScene("Deck Building");
    }

    public void OpenTutorialMainMenuScene()
    {
        OpenScene("Tutorial Main Menu");
    }

    public void OpenScene(string name)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        SceneManager.LoadSceneAsync(name);
    }
}
