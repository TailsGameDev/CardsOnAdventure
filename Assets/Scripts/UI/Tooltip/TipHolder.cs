using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipHolder : PopUpOpener
{
    [SerializeField]
    private string title = null;

    [SerializeField]
    private TooltipSectionData[] sectionsData = null;

    public void OpenTipPopUp()
    {
        popUpOpener.OpenTooltipPopUp(sectionsData, title);
    }
}
