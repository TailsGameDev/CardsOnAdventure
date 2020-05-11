using System;
using UnityEngine;

public class TipReceptor : DragAndDropReceptor
{
    [SerializeField]
    protected string title = null;

    [SerializeField]
    protected TipSectionData[] tipData = null;

    private void Awake()
    {
        for (int i = 0; i < tipData.Length; i++)
        {
            tipData[i].message = tipData[i].message.Replace("<br>", "\n");
        }
    }

    public override Type GetDragAndDropReceptorType()
    {
        return typeof(TipDragAndDrop);
    }

    public virtual void OpenTip(TipPopUpOpener tipPopUpOpener)
    {
        tipPopUpOpener.OpenTipPopUp(tipData, title);
    }

    public override void OnDroppedInReceptor()
    {
        // NOTE: the following line was substituted by OpenTip Method. This way receptors does not need opener attribute
        // ((TipPopUpOpener)popUpOpener).OpenTipPopUp(tipData, title);
    }
}