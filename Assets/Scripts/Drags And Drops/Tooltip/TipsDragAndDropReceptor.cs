using System;
using UnityEngine;

public class TipsDragAndDropReceptor : DragAndDropReceptor
{
    [SerializeField]
    protected string title = null;

    [SerializeField]
    protected TipSectionData[] tipData = null;

    [SerializeField]
    protected TipsPopUpOpener popUpOpener = null;

    private void Awake()
    {
        for (int i = 0; i < tipData.Length; i++)
        {
            tipData[i].message = tipData[i].message.Replace("<br>", "\n");
        }
    }

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