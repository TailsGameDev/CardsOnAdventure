using System;
using UnityEngine;
using UnityEngine.UI;

public class TipReceptor : DragAndDropReceptor
{
    [SerializeField]
    protected string title = null;

    [SerializeField]
    protected TipSectionData[] tipData = null;

    protected static GameObject heyImATipReceptor;
    protected GameObject myHeyImATipReceptor;

    public static GameObject HeyImATipReceptor { get => heyImATipReceptor; set => heyImATipReceptor = value; }
    public string Title { get => title; }

    private void Awake()
    {
        for (int i = 0; i < tipData.Length; i++)
        {
            tipData[i].message = tipData[i].message.Replace("<br>", "\n");
        }
    }

    private void OnEnable()
    {
        TipDragAndDrop.onDrag += HeyImHereImATipReceptor;
        TipDragAndDrop.onDrop += TurnSignalOff;
    }
    private void OnDisable()
    {
        TipDragAndDrop.onDrag -= HeyImHereImATipReceptor;
        TipDragAndDrop.onDrop -= TurnSignalOff;
    }

    private void HeyImHereImATipReceptor()
    {
        myHeyImATipReceptor = Instantiate(heyImATipReceptor, transform.position, Quaternion.identity);
        myHeyImATipReceptor.transform.SetParent(transform, false);
        myHeyImATipReceptor.transform.rotation = Quaternion.identity;
    }
    private void TurnSignalOff()
    {
        Destroy(myHeyImATipReceptor);
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