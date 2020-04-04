using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipHolder : PopUpOpener
{
    [SerializeField]
    protected string title = null;

    [SerializeField]
    protected TipSectionData[] tipData = null;

    public void OpenTipPopUp()
    {
        popUpOpener.OpenTooltipPopUp(tipData, title);
    }
}
