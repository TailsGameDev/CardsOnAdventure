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
        Open(title: "Are You sure?", warningMessage, "I'm Sure", "Cancel", onConfirm, () => { });
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
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, () => { });
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
        Open(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm, () => { });
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
}
