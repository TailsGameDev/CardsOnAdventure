using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipsDragAndDrop : DragAndDrop
{
    [SerializeField]
    private Text text = null;

    [SerializeField]
    private GameObject speakBaloon = null;

    private static GameObject staticSpeakBalloon;

    protected void Awake()
    {
        if (staticSpeakBalloon == null)
        {
            staticSpeakBalloon = speakBaloon;
        }
        StartCoroutine(MakeFontNormalInsistently());
    }

    IEnumerator MakeFontNormalInsistently()
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
    }
    protected override void OnEnteredAReceptor(DragAndDropReceptor receptor)
    {
        text.fontStyle = FontStyle.BoldAndItalic;
    }

    protected override Type GetDragAndDropType()
    {
        return typeof(TipsDragAndDrop);
    }

    protected override void OnExitedAllReceptors()
    {
        text.fontStyle = FontStyle.Italic;
    }

    protected override void BeforeDrop()
    {
    }

    protected override void OnDroppedSpecificBehaviour()
    {
        base.ReturnToOriginalPosition();
        StartCoroutine(MakeFontNormalInsistently());
    }
}
