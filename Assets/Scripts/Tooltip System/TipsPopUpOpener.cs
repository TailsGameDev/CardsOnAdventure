using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsPopUpOpener : OpenersSuperclass
{
    public void OpenTipsPopUp(TipSectionData[] tipData, string title)
    {
        openerOfPopUpsMadeInEditor.OpenTooltipPopUp(tipData, title);
    }
}
