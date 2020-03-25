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
    private Button confirmBtn = null;

    #region Sound
    [SerializeField]
    private AudioClip mapBGM = null;

    [SerializeField]
    private AudioClip clickBtnSFX = null;
    #endregion

    public delegate void OnConfirmBtnClicked();

    private void Awake()
    {
        if (popUpOpener == null)
        {
            popUpOpener = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    #region Open XXX PopUp

    public void OpenPausePopUp()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
        pausePopUp.SetActive(true);
    }

    public void OpenRulesPopUp()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
        rulesPopUp.SetActive(true);
    }

    public void OpenLoadingPopUp()
    {
        loadingPopUp.SetActive(true);
    }

    public void OpenSettingsPopUp()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
        settingsPopUp.SetActive(true);
    }

    public void OpenMapPopUp()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
        mapPopUp.SetActive(true);
    }

    #endregion

    public void CloseMapPopUp()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
        mapPopUp.SetActive(false);
    }

    public void PlayBtnSound()
    {
        SoundManager.Instance.PlaySFX(clickBtnSFX);
    }

    public void OpenConfirmationRequestPopUp(string warningMessage, OnConfirmBtnClicked onConfirm)
    {
        OpenCustomPopUp(title: "Are You sure?",warningMessage,"I'm Sure", "Cancel", onConfirm);
    }

    public void OpenCustomPopUp(string title, string warningMessage, string confirmBtnMessage, string cancelBtnMessage, OnConfirmBtnClicked onConfirm, AudioClip bgm)
    {
        SoundManager.Instance.PlayBGM(bgm);
        OpenCustomPopUp(title, warningMessage, confirmBtnMessage, cancelBtnMessage, onConfirm);
    }

    public void OpenCustomPopUp(string title, string warningMessage, string confirmBtnMessage, string cancelBtnMessage, OnConfirmBtnClicked onConfirm)
    {
        customPopUpTitle.text = title;

        warningText.text = warningMessage;

        customConfirmText.text = confirmBtnMessage;

        customCancelText.text = cancelBtnMessage;

        confirmBtn.onClick.AddListener(delegate {
            customPopUp.SetActive(false);
            SoundManager.Instance.PlaySFX(clickBtnSFX);
            onConfirm();
        });

        customPopUp.SetActive(true);
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
