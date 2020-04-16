using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : UICardsHolderEventHandler
{
    [SerializeField]
    private PausePopUpOpener pausePopUpOpener = null;

    public void OnPauseBtnClicked()
    {
        pausePopUpOpener.OpenPausePopUp();
    }
}