using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TipsDragAndDrop : DragAndDrop
{

    [SerializeField]
    private Text text = null;

    protected void Awake()
    {
        StartCoroutine(MakeFontNormalInsistently());
    }

    IEnumerator MakeFontNormalInsistently()
    {
        yield return null;
        text.fontStyle = FontStyle.Normal;
        yield return null;
        text.fontStyle = FontStyle.Normal;
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
        StartCoroutine(MakeFontNormalInsistently());
    }
}
