using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpener : PopUpOpener
{
    // Start is called before the first frame update
    public void OpenMainMenuScene()
    {
        OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    public void OpenScene(string name)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        SceneManager.LoadSceneAsync(name);
    }
}
