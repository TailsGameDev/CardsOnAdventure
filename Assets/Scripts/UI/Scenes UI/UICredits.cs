using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICredits : MonoBehaviour
{
    [SerializeField]
    private PauseMenu sceneOpener = null;

    public void OnCloseBtnClicked()
    {
        sceneOpener.OpenScene("Main Menu");
    }
}
