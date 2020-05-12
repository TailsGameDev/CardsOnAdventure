using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipDragAndDrop : DragAndDrop
{
    [SerializeField]
    private TipPopUpOpener tipPopUpOpener = null;

    [SerializeField]
    private Text text = null;

    [SerializeField]
    private GameObject tipSignal = null;

    [SerializeField]
    private GameObject speakBaloon = null;

    private static GameObject staticSpeakBalloon;

    public delegate void OnDragOrDrop();
    public static event OnDragOrDrop onDrag;
    public static event OnDragOrDrop onDrop;

    protected void Awake()
    {
        if (staticSpeakBalloon == null)
        {
            staticSpeakBalloon = speakBaloon;
        }
        StartCoroutine(MakeFontNormalInsistently());
    }

    private void Start()
    {
        if (TipReceptor.HeyImATipReceptor == null)
        {
            TipReceptor.HeyImATipReceptor = tipSignal;
        }
    }

    private IEnumerator MakeFontNormalInsistently()
    {
        yield return null;
        text.fontStyle = FontStyle.Normal;
        yield return null;
        text.fontStyle = FontStyle.Normal;
    }

    public static void AskToUseTips()
    {
        if (Settings.ShouldAskForTips())
        {
            staticSpeakBalloon.SetActive(true);
        }
    }

    protected override void OnStartDragging()
    {
        text.fontStyle = FontStyle.Italic;
        onDrag?.Invoke();
    }
    protected override void OnEnteredAReceptor(DragAndDropReceptor receptor)
    {
        text.fontStyle = FontStyle.BoldAndItalic;
    }

    protected override Type GetDragAndDropType()
    {
        return typeof(TipDragAndDrop);
    }

    protected override void OnExitedAllReceptors()
    {
        text.fontStyle = FontStyle.Italic;
    }

    protected override void BeforeDrop()
    {
        onDrop?.Invoke();
        if (this.receptor != null)
        {
            ((TipReceptor)this.receptor).OpenTip(tipPopUpOpener);
        }
    }

    protected override void OnDroppedSpecificBehaviour()
    {
        base.ReturnToOriginalPosition();
        StartCoroutine(MakeFontNormalInsistently());
    }
}
