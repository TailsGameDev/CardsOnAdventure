using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThePopUpOpenerInstance : OpenersSuperclass
{

    [SerializeField]
    private GameObject pausePopUp = null;

    [SerializeField]
    private GameObject loadingPopUp = null;

    [SerializeField]
    private GameObject settingsPopUp = null;

    [SerializeField]
    private GameObject tipPopUp = null;

    [SerializeField]
    private Text tipPopUpTitle = null;

    [SerializeField]
    private string battleRules = null;
    [SerializeField]
    private Sprite battleRulesSprite = null;

    [SerializeField]
    private GameObject customPopUp = null;

    private Stack<GameObject> popUpsStack = new Stack<GameObject>();

    public delegate void OnBtnClicked();

    private void Awake()
    {
        if (openerOfPopUpsMadeInEditor == null)
        {
            openerOfPopUpsMadeInEditor = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            // Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", defaultValue: 0) == 0;

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
    public bool customPopUpIsClosed()
    {
        return ! customPopUp.activeSelf;
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
    public void OpenBattleRulesPopUp()
    {
        TipSectionData[] battleRulesContent = new TipSectionData[]
        {
            new TipSectionData(battleRulesSprite, 100.0f),
            new TipSectionData(battleRules, 3050.0f)
        };

        OpenTipPopUp( battleRulesContent, title: "Battle Rules" );
    }
    public void SetLoadingPopUpActiveToTrue()
    {
        loadingPopUp.SetActive(true);
    }
    public void OpenSettingsPopUp()
    {
        OpenPopUp(settingsPopUp);
    }
    public void OpenMapScene()
    {
        SetLoadingPopUpActiveToTrue();
        CloseAllPopUpsExceptLoading();
        SceneManager.LoadScene("Map");
    }
    public void OpenCustomPopUp()
    {
        if (!customPopUp.activeSelf)
        {
            OpenPopUp(customPopUp);
        }
        else
        {
            L.ogWarning(this,"Called OpenCustomPopUp but it was already opened");
        }
    }
    public void OpenTipPopUp(TipSectionData[] tipsData, string title)
    {
        tipPopUpTitle.text = title;
        tipPopUp.GetComponent<TipPopUp>().PopulateAllSections(tipsData);
        OpenPopUp(tipPopUp);
    }
    #endregion
}
