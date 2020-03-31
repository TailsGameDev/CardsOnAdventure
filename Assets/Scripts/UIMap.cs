using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMap : PopUpOpener
{
    [SerializeField]
    private Spot finalSpot = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip winSound = null;

    private bool initialized = false;

    private void Awake()
    {
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        if (!initialized)
        {
            ResetMap();
            initialized = true;
        }
    }

    public void ResetMap()
    {
        finalSpot.ResetMap();
    }

    public void UpdateMap()
    {
        InitializeIfNeeded();
        finalSpot.UpdateMap();
    }

    #region On Button Clicks
    public void OnSimpleBattleClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnDifficultBattleClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnBossBattleClicked(Transform btnTransform)
    {
        audioRequisitor.RequestBGM(winSound);
        popUpOpener.OpenCustomPopUp("You Beat the game!!!", "You are Awesome!","Go to Menu","Look the Map", GoToMenu);
        //ClearSpot(btnTransform);
        //BattleDefaultBehaviour();
    }

    void GoToMenu()
    {
        sceneOpener.OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    public void OnMageMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnRougueMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    #endregion

    private void BattleDefaultBehaviour()
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        popUpOpener.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenScene("Battle");
    }

    void ClearSpot(Transform btnTransform)
    {
        Spot spot = btnTransform.parent.GetComponent<Spot>();
        if (spot == null)
        {
            Debug.LogError("[UIMap] spot is null.", this);
        }
        spot.Cleared = true;
    }

    public void OnPauseMenuOppenerBtnClick()
    {
        popUpOpener.OpenPausePopUp();
    }
}
