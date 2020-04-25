using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private GameObject tipsPopUp = null;

    [SerializeField]
    private Text tooltipPopUpTitle = null;

    [SerializeField]
    private string battleRules = null;
    [SerializeField]
    private Sprite battleRulesSprite = null;

    [SerializeField]
    private GameObject customPopUp = null;

    private Stack<GameObject> popUpsStack = new Stack<GameObject>();

    #region Sound
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip mapBGM = null;
    #endregion

    public delegate void OnBtnClicked();

    private void Awake()
    {
        if (popUpOpener == null)
        {
            popUpOpener = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Screen.fullScreen = true;

            battleRules = battleRules.Replace("<br>", "\n");
        } else
        {
            Destroy(gameObject);
        }
    }

    #region Operations involving the stack
    void LogStack()
    {
        /*
        string names = "";
        foreach (GameObject p in popUpsStack)
        {
            names += " " + p.name;
        }
        Debug.Log("[ThePopUpOpenerInstance] " + names, this);
        */
    }

    private void OpenPopUp(GameObject popUp)
    {
        popUp.SetActive(true);
        foreach (GameObject p in popUpsStack)
        {
            p.SetActive(false);
        }
        popUpsStack.Push(popUp);
         LogStack();
    }

    // Called by editor
    public void IfThereIsAPopUpOnTopThenCloseIt()
    {
        if (popUpsStack.Count > 0)
        {
            GameObject popUpOnTop = popUpsStack.Pop();
            if (popUpsStack.Count > 0)
            {
                GameObject secondOnTop = popUpsStack.Peek();
                secondOnTop.SetActive(true);
            }
            popUpOnTop.SetActive(false);
        }
         LogStack();
    }

    public void CloseAllPopUpsExceptLoading()
    {
        while (popUpsStack.Count > 0)
        {
            popUpsStack.Pop().SetActive(false);
        }
         LogStack();
    }

    public bool AllPopUpsAreClosed()
    {
        return popUpsStack.Count == 0;
    }

    #endregion

    #region Open XXX PopUp

    public void OpenPausePopUp()
    {
        OpenPopUp(pausePopUp);
    }

    public void OpenWhatTheTeachingSpiritIsPopUp()
    {
        OpenPopUp(rulesPopUp);
    }

    public void OpenBattleRulesPopUp()
    {
        TipSectionData[] battleRulesContent = new TipSectionData[]
        {
            new TipSectionData(battleRulesSprite, 100.0f),
            new TipSectionData(battleRules, 2300.0f)
        };

        OpenTooltipPopUp( battleRulesContent, title: "Battle Rules" );
    }

    public void SetLoadingPopUpActiveToTrue()
    {
        loadingPopUp.SetActive(true);
    }

    public void OpenSettingsPopUp()
    {
        OpenPopUp(settingsPopUp);
    }

    public void OpenMapPopUp()
    {
        audioRequisitor.RequestBGM(mapBGM);
        SetLoadingPopUpActiveToTrue();
        CloseAllPopUpsExceptLoading();
        SceneManager.LoadScene("Map");
    }

    public void OpenCustomPopUp()
    {
        OpenPopUp(customPopUp);
    }
    
    public void OpenTooltipPopUp(TipSectionData[] tipsData, string title)
    {
        tooltipPopUpTitle.text = title;
        tipsPopUp.GetComponent<TipsPopUp>().PopulateAllSections(tipsData);
        OpenPopUp(tipsPopUp);
    }
    #endregion
}
