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

    void GoToMenu()
    {
        sceneOpener.OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    #region On Battle Spot Buttons Clicked

        public void OnSimpleBattleClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            ClearSpot(btnTransform);
            DeckPrototypeFactory.PrepareRandomDeckForTheEnemy();
            SetUpBattleAndOpenIt();
        }

        public void OnToughBattleClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            ClearSpot(btnTransform);
            DeckPrototypeFactory.PrepareToughRandomDeckForTheEnemy();
            SetUpBattleAndOpenIt();
        }

        // the btnTransform parameter might be used if we substitute the final spot for some battle
        public void OnBossBattleClicked(Transform btnTransform)
        {
            audioRequisitor.RequestBGM(winSound);
            customPopUpOpener.Open("You Beat the game!!!", "You are Awesome!","Go to Menu","Look the Map", GoToMenu);
            //ClearSpot(btnTransform);
            //BattleDefaultBehaviour();
        }

    #endregion

    #region On Master Buttons Clicked

        public void OnMageMasterClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            DeckPrototypeFactory.PrepareMageDeckForTheEnemy();
            ClearSpot(btnTransform);
            SetUpBattleAndOpenIt();
        }

        public void OnWarriorMasterClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            DeckPrototypeFactory.PrepareWarriorDeckForTheEnemy();
            ClearSpot(btnTransform);
            SetUpBattleAndOpenIt();
        }

        public void OnRougueMasterClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            DeckPrototypeFactory.PrepareRogueDeckForTheEnemy();
            ClearSpot(btnTransform);
            SetUpBattleAndOpenIt();
        }

        public void OnGuardianMasterClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            DeckPrototypeFactory.PrepareGuardianDeckForTheEnemy();
            ClearSpot(btnTransform);
            SetUpBattleAndOpenIt();
        }

    #endregion

    private void SetUpBattleAndOpenIt()
    {
        DeckPrototypeFactory.PrepareRandomDeckForThePlayer();
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
