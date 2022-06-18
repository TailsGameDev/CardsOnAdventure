using UnityEngine;
using UnityEngine.UI;

public class CustomPopUp : OpenersSuperclass
{
    [SerializeField]
    private Text customPopUpTitle = null;

    [SerializeField]
    private Text warningText = null;

    [SerializeField]
    private Text customConfirmText = null;

    [SerializeField]
    private Text customCancelText = null;

    [SerializeField]
    private UICustomBtn customConfirmBtn = null;
    [SerializeField]
    private UICustomBtn customCancelBtn = null;

    [SerializeField]
    private UIBtn closeBtn = null;

    [SerializeField]
    private CustomPopUpCardsHolder cardsDisplay = null;

    private bool btnsActive = true;

    public delegate void OnBtnClicked();

    [SerializeField]
    private bool isIncidentPopUp = false;

    private void Awake()
    {
        if (isIncidentPopUp)
        {
            if (incidentPopUpOpener == null)
            {
                incidentPopUpOpener = this;
            }
        }
        else
        {
            if (customPopUpOpener == null)
            {
                customPopUpOpener = this;
            }
        }
    }

    public void ClosePopUpOnTop()
    {
        openerOfPopUpsMadeInEditor.IfThereIsAPopUpOnTopThenCloseIt();
    }

    public void OpenConfirmationRequestPopUp(string warningMessage, OnBtnClicked onConfirm)
    {
        Open(title: "Are You sure?", warningMessage, "I'm Sure", "Cancel", onConfirm, DefaultCancelBehaviour);
    }

    private void DefaultCancelBehaviour()
    {
        openerOfPopUpsMadeInEditor.IfThereIsAPopUpOnTopThenCloseIt();
    }

    public void OpenMessageIfNoCustomPopUpIsOpenned(string title, string warningMessage)
    {
        if (openerOfPopUpsMadeInEditor.customPopUpIsClosed())
        {
            btnsActive = false;
            Open(title, warningMessage, " ", " ", ()=> { }, ()=> { });
            btnsActive = true;
        }
    }

    public void OpenWithBGM // With Custom Cancel / With BGM
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnBtnClicked onConfirm,
            OnBtnClicked onCancel,
            PreMadeAudioRequest openingBGM
        )
    {
        openingBGM.RequestPlaying();
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, onCancel);
    }

    public void OpenDisplayingUnblockedCardsOfClass
    (
        string title,
        string warningMessage,
        string confirmBtnMessage,
        OnBtnClicked onConfirm,
        PreMadeAudioRequest openingBGM,
        Classes classeOfCardsToShow
    )
    {
        openingBGM.RequestPlaying();
        cardsDisplay.ShowUnlockedCardsOfClass(classeOfCardsToShow);
        OpenAndMakeUncloseable(title, warningMessage, confirmBtnMessage, 
            "",
            onConfirm: ()=> { cardsDisplay.ClearAttributes(); onConfirm(); },
            onCancel: () => {}
            );
        customCancelBtn.gameObject.SetActive(false);
    }

    public void OpenDisplayingCardsToChoose
    (
        string title,
        string warningMessage,
        string confirmBtnMessage,
        string cancelBtnMessage,
        OnBtnClicked onConfirm,
        OnBtnClicked onCancel,
        PreMadeAudioRequest openingBGM,
        Card[] cards
    )
    {
        openingBGM.RequestPlaying();
        cardsDisplay.ShowCards(cards);
        OpenAndMakeUncloseable(title, warningMessage, confirmBtnMessage,
            cancelBtnMessage,
            onConfirm: () => { onConfirm(); cardsDisplay.ClearAttributes(); },
            onCancel: () => { onCancel(); cardsDisplay.ClearAttributes(); }
            );
    }
    public void OpenDisplayingRewardCards
    (
        string title,
        string warningMessage,
        string confirmBtnMessage,
        OnBtnClicked onConfirm,
        PreMadeAudioRequest openingBGM,
        Card[] cards
    )
    {
        openingBGM.RequestPlaying();
        cardsDisplay.ShowCards(cards);
        OpenAndMakeUncloseable(title, warningMessage, confirmBtnMessage,
            "",
            onConfirm: () => { onConfirm(); cardsDisplay.ClearAttributes(); },
            onCancel: () => {}
            );
        customCancelBtn.gameObject.SetActive(false);
    }

    public void OpenAndMakeUncloseable
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnBtnClicked onConfirm,
            OnBtnClicked onCancel
        )
    {
        Open(
                title,
                warningMessage,
                confirmBtnMessage,
                cancelBtnMessage,
                onConfirm: () => { closeBtn.gameObject.SetActive(true); onConfirm(); },
                onCancel: () => { closeBtn.gameObject.SetActive(true); onCancel(); }
            );
        closeBtn.gameObject.SetActive(false);
    }

    public void Open // With Custom Cancel / Without BGM
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnBtnClicked onConfirm,
            OnBtnClicked onCancel
        )
    {
        customPopUpTitle.text = title;

        warningText.text = warningMessage;

        customConfirmText.text = confirmBtnMessage;
        customConfirmBtn.onClicked = onConfirm;
        customConfirmBtn.gameObject.SetActive( btnsActive );

        customCancelText.text = cancelBtnMessage;
        customCancelBtn.onClicked = onCancel;
        customCancelBtn.gameObject.SetActive( btnsActive );

        if (isIncidentPopUp)
        {
            openerOfPopUpsMadeInEditor.OpenIncidentPopUp();
        }
        else
        {
            openerOfPopUpsMadeInEditor.OpenCustomPopUp();
        }
    }

    public void DeactivateCancel()
    {
        customCancelBtn.gameObject.SetActive(false);
    }
}
