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

    private void Awake()
    {
        if (customPopUpOpener == null)
        {
            customPopUpOpener = this;
        }
    }

    public void OpenConfirmationRequestPopUp(string warningMessage, OnBtnClicked onConfirm)
    {
        Open(title: "Are You sure?", warningMessage, "I'm Sure", "Cancel", onConfirm, DefaultCancelBehaviour);
    }

    public void OpenWithNoBtns(string title, string warningMessage)
    {
        btnsActive = false;
        Open(title, warningMessage, " ", " ", ()=> { }, ()=> { });
        btnsActive = true;
    }

    public void Open // with BGM
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnBtnClicked onConfirm,
            PreMadeSoundRequest openingBGM
        )
    {
        openingBGM.RequestPlaying();
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, DefaultCancelBehaviour);
    }

    public void Open // With Default Cancel
    (
        string title,
        string warningMessage,
        string confirmBtnMessage,
        string cancelBtnMessage,
        OnBtnClicked onConfirm
    )
    {
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, DefaultCancelBehaviour);
    }

    public void Open // With Custom Cancel / With BGM
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnBtnClicked onConfirm,
            OnBtnClicked onCancel,
            PreMadeSoundRequest openingBGM
        )
    {
        openingBGM.RequestPlaying();
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, onCancel);
    }

    public void Open // With Custom Cancel / With BGM
    (
        string title,
        string warningMessage,
        string confirmBtnMessage,
        string cancelBtnMessage,
        OnBtnClicked onConfirm,
        OnBtnClicked onCancel,
        PreMadeSoundRequest openingBGM,
        Classes classeOfCardsToShow
    )
    {
        openingBGM.RequestPlaying();
        cardsDisplay.ShowCardsOfClass(classeOfCardsToShow);
        OpenAndMakeUncloseable(title, warningMessage, confirmBtnMessage, 
            cancelBtnMessage,
            onConfirm: ()=> { cardsDisplay.ClearAttributes(); onConfirm(); },
            onCancel: () => { cardsDisplay.ClearAttributes(); onCancel(); }
            );
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

        openerOfPopUpsMadeInEditor.OpenCustomPopUp();
    }

    private void DefaultCancelBehaviour()
    {
        openerOfPopUpsMadeInEditor.IfThereIsAPopUpOnTopThenCloseIt();
    }

    public void ClosePopUpOnTop()
    {
        openerOfPopUpsMadeInEditor.IfThereIsAPopUpOnTopThenCloseIt();
    }
}
