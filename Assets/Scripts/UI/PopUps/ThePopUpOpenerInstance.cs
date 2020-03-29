using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThePopUpOpenerInstance : PopUpOpener
{

    [SerializeField]
    private GameObject pausePopUp = null;

    [SerializeField]
    private GameObject rulesPopUp = null;

    [SerializeField]
    private GameObject loadingPopUp = null;

    [SerializeField]
    private GameObject settingsPopUp = null;

    [SerializeField]
    private GameObject mapPopUp = null;

    [SerializeField]
    private GameObject customPopUp = null;

    [SerializeField]
    private Text customPopUpTitle = null;

    [SerializeField]
    private Text warningText = null;

    [SerializeField]
    private Text customConfirmText = null;

    [SerializeField]
    private Text customCancelText = null;

    [SerializeField]
    private UICustomBtn confirmBtn = null;

    #region Sound
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip mapBGM = null;
    #endregion

    public delegate void OnConfirmBtnClicked();

    private void Awake()
    {
        if (popUpOpener == null)
        {
            popUpOpener = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Screen.fullScreen = true;
        } else
        {
            Destroy(gameObject);
        }

        confirmBtn.onClicked += CloseCustomPopUp;
    }

    #region Open XXX PopUp

    public void OpenPausePopUp()
    {
        pausePopUp.SetActive(true);
    }

    public void OpenRulesPopUp()
    {
        rulesPopUp.SetActive(true);
    }

    public void OpenLoadingPopUp()
    {
        loadingPopUp.SetActive(true);
    }

    public void OpenSettingsPopUp()
    {
        settingsPopUp.SetActive(true);
    }

    public void OpenMapPopUp()
    {
        audioRequisitor.RequestBGM(mapBGM);
        mapPopUp.SetActive(true);
    }

    #endregion

    public void CloseMapPopUp()
    {
        mapPopUp.SetActive(false);
    }

    public void OpenConfirmationRequestPopUp(string warningMessage, OnConfirmBtnClicked onConfirm)
    {
        OpenCustomPopUp(title: "Are You sure?",warningMessage,"I'm Sure", "Cancel", onConfirm);
    }

    public void OpenCustomPopUp
        (
            string title, 
            string warningMessage, 
            string confirmBtnMessage, 
            string cancelBtnMessage, 
            OnConfirmBtnClicked onConfirm, 
            PreMadeSoundRequest openingBGM
        )
    {
        openingBGM.RequestPlaying();
        OpenCustomPopUp(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm);
    }

    public void OpenCustomPopUp
        (
            string title,
            string warningMessage,
            string confirmBtnMessage,
            string cancelBtnMessage,
            OnConfirmBtnClicked onConfirm
        )
    {
        customPopUpTitle.text = title;

        warningText.text = warningMessage;

        customConfirmText.text = confirmBtnMessage;

        customCancelText.text = cancelBtnMessage;

        confirmBtn.onClicked += onConfirm;

        customPopUp.SetActive(true);
    }

    private void CloseCustomPopUp()
    {
        customPopUp.SetActive(false);
    }

    public void UpdateMap()
    {
        mapPopUp.GetComponent<UIMap>().UpdateMap();
    }

    public void ResetMap()
    {
        mapPopUp.GetComponent<UIMap>().ResetMap();
    }
}
