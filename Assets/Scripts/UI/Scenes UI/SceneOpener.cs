using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpener : OpenersSuperclass
{
    private void Awake()
    {
        if (sceneOpener == null)
        {
            sceneOpener = this;
        }
    }

    public void OpenBattle()
    {
        OpenScene("Battle");
    }

    public void OpenMapScene()
    {
        OpenScene("Map");
    }

    // Start is called before the first frame update
    public void OpenMainMenu()
    {
        OpenScene("Main Menu");
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
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
        StartCoroutine(OpenSceneCoroutine(name));
    }

    private IEnumerator OpenSceneCoroutine(string name)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        yield return null;
        SceneManager.LoadSceneAsync(name);
    }
}
