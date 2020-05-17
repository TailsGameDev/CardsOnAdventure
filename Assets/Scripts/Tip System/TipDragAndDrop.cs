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
    [SerializeField]
    private GameObject bonusRepositioningTip = null;
    [SerializeField]
    private GameObject placeCardsNowTip = null;

    private static GameObject staticSpeakBalloon;
    private static GameObject staticBonusRepositioningTip;
    private static GameObject staticPlaceCardsNowTip;

    public delegate void OnDragOrDrop();
    public static event OnDragOrDrop onDrag;
    public static event OnDragOrDrop onDrop;

    protected void Awake()
    {
        if (staticSpeakBalloon == null)
        {
            staticSpeakBalloon = speakBaloon;
        }
        if (staticBonusRepositioningTip == null)
        {
            staticBonusRepositioningTip = bonusRepositioningTip;
        }
        if (staticPlaceCardsNowTip == null)
        {
            staticPlaceCardsNowTip = placeCardsNowTip;
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

    public static void ShowBonusRepositioningTip()
    {
        if (Settings.ShouldAskForTips())
        {
            staticBonusRepositioningTip.SetActive(true);
        }
    }
    public static void ShowPlaceCardsNowTip()
    {
        if (Settings.ShouldAskForTips())
        {
            staticPlaceCardsNowTip.SetActive(true);
        }
    }

    public static void HideBonusRepositioningTip()
    {
        staticBonusRepositioningTip.SetActive(false);
    }
    public static void HidePlaceCardsNowTip()
    {
        staticPlaceCardsNowTip.SetActive(false);
    }

    protected override void OnStartDragging()
    {
        staticSpeakBalloon.SetActive(false);
        text.fontStyle = FontStyle.Italic;
        onDrag?.Invoke();
    }
    protected override void OnEnteredAReceptor(DragAndDropReceptor receptor)
    {
        bool isTheTipOfHowToUseTips = ((TipReceptor)receptor).Title == text.text;
        if (!isTheTipOfHowToUseTips)
        {
            text.fontStyle = FontStyle.Bold;
        }
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
