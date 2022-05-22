using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPopUpOpener : OpenersSuperclass
{
    [SerializeField] private Text dialogText = null;

    private void Awake()
    {
        if (storyPopUpOpener == null) 
        {
            storyPopUpOpener = this;
        }
    }

    public void Open(string story)
    {
        dialogText.text = story;
        openerOfPopUpsMadeInEditor.OpenStoryPopUp();
    }

    public void OnCloseButtonClick()
    {
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenMapScene();
    }
}
