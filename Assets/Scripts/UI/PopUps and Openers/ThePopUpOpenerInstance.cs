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
    private Settings settingsPopUp = null;

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

    [SerializeField]
    private GameObject incidentPopUp =  null;

    [SerializeField]
    private GameObject storyPopUp = null;

    [SerializeField]
    private GameObject tipSystem = null;

    private Stack<GameObject> popUpsStack = new Stack<GameObject>();

    public delegate void OnBtnClicked();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnLoaded;
        if (openerOfPopUpsMadeInEditor == null)
        {
            openerOfPopUpsMadeInEditor = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            battleRules = battleRules.Replace("<br>", "\n");
        } else
        {
            Destroy(gameObject);
        }
    }

    void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        tipSystem.SetActive(true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoaded;
    }

    private void Start()
    {
        settingsPopUp.RefreshCursor();
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

            if (popUpOnTop == pausePopUp)
            {
                TimeFacade.TimeIsStopped = false;
            }
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
            IfThereIsAPopUpOnTopThenCloseIt();
        }
    }
    public bool AllPopUpsAreClosed()
    {
        return popUpsStack.Count == 0;
    }
    #endregion

    #region Open XXX PopUp
    public void OpenPausePopUp()
    {
        TimeFacade.TimeIsStopped = true;
        OpenPopUp(pausePopUp);
    }
    public void OpenBattleRulesPopUp()
    {
        TipSectionData[] battleRulesContent = new TipSectionData[]
        {
            new TipSectionData(battleRulesSprite, 230.0f),
            new TipSectionData(battleRules, 2300.0f)
        };

        OpenTipPopUp( battleRulesContent, title: "Battle Rules" );
    }
    public void SetLoadingPopUpActiveToTrueAndDeactivateTips()
    {
        loadingPopUp.SetActive(true);
        tipSystem.SetActive(false);
    }
    public void OpenSettingsPopUp()
    {
        OpenPopUp(settingsPopUp.GetGameObject());
    }
    public void OpenMapScene()
    {
        SetLoadingPopUpActiveToTrueAndDeactivateTips();
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
    public void OpenIncidentPopUp()
    {
        if (!incidentPopUp.activeSelf)
        {
            OpenPopUp(incidentPopUp);
        }
        else
        {
            L.ogWarning(this, "Called OpenIncidentPopUp but it was already opened");
        }
    }
    public void OpenTipPopUp(TipSectionData[] tipsData, string title)
    {
        tipPopUpTitle.text = title;
        tipPopUp.GetComponent<TipPopUp>().PopulateAllSections(tipsData);
        OpenPopUp(tipPopUp);
    }
    public void OpenStoryPopUp()
    {
        if (!storyPopUp.activeSelf)
        {
            OpenPopUp(storyPopUp);
        }
        else
        {
            L.ogWarning(this, "Called OpenStoryPopUp but it was already opened");
        }
    }
    #endregion
}
