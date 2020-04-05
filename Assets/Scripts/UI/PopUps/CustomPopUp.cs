using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPopUp : PopUpOpener
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
        Open( title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, onCancel );
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
                onConfirm: ()=> { closeBtn.gameObject.SetActive(true); onConfirm(); },
                onCancel: ()=> { closeBtn.gameObject.SetActive(true); onCancel(); }
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

        customCancelText.text = cancelBtnMessage;

        customConfirmBtn.onClicked = null;
        customConfirmBtn.onClicked += onConfirm;

        customCancelBtn.onClicked = null;
        customCancelBtn.onClicked += onCancel;

        popUpOpener.OpenCustomPopUp();
    }

    private void DefaultCancelBehaviour()
    {
        popUpOpener.IfThereIsAPopUpOnTopThenCloseIt();
    }

    public void ClosePopUpOnTop()
    {
        popUpOpener.IfThereIsAPopUpOnTopThenCloseIt();
    }
}
