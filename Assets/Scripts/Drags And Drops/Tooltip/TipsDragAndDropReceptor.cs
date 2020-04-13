using System;
using UnityEngine;

public class TipsDragAndDropReceptor : DragAndDropReceptor
{
    [SerializeField]
    protected string title = null;

    [SerializeField]
    protected TipSectionData[] tipData = null;

    [SerializeField]
    protected TipPopUpOpener popUpOpener = null;

    public override Type GetDragAndDropReceptorType()
    {
        return typeof(TipsDragAndDrop);
    }

    public override void OnDroppedInReceptor()
    {
        OpenTipPopUp();
    }

    private void OpenTipPopUp()
    {
        popUpOpener.OpenTipsPopUp(tipData, title);
    }
}