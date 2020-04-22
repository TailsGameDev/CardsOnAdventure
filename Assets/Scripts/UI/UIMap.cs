using System.Collections.Generic;
using UnityEngine;

public class UIMap : PopUpOpener
{
    [SerializeField]
    private Spot[] finalSpotForEachMap = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip winSound = null;

    [SerializeField]
    private MapsCacheGetter mapsCache = null;

    private void Awake()
    {
        mapsCache.UpdateWithPlayerProgress();

        if (mapsCache.StartOfMatch)
        {
            BuildAllMaps();
        }
        else if ( DataStructuresAreEmpty() )
        {
            bool succeed = mapsCache.TryToLoadAllMapsDataFromDeviceStorage(finalSpotForEachMap);

            if (succeed)
            {
                CopyDataFromAttributesToSceneSpots();
            }
            else
            {
                BuildAllMaps();
            }
        }
        else // Player is in the middle of the game and the data should be fresh in mapsCache
        {
            CopyDataFromAttributesToSceneSpots();
        }
    }

    private void BuildAllMaps()
    {
        mapsCache.StartOfMatch = false;

        mapsCache.ClearRootsAndSpots();

        foreach (Spot mapFinalSpot in finalSpotForEachMap)
        {
            BuildMapAndSaveItsDataInMapsCache(mapFinalSpot);
        }
    }

    private void BuildMapAndSaveItsDataInMapsCache(Spot mapFinalSpot)
    {
        string mapName = mapFinalSpot.MapName;
        mapFinalSpot.BuildFromZero();
        List<SpotInfo> allSpotsInfo = mapFinalSpot.GetInfo(out int rootIndex);
        mapsCache.CacheRootsAndSpotsOfSingleMap(allSpotsInfo, rootIndex, mapName);
    }

    private bool DataStructuresAreEmpty()
    {
        return mapsCache.DataStructuresAreEmpty();
    }

    public void CopyDataFromAttributesToSceneSpots()
    {
        foreach (Spot root in finalSpotForEachMap)
        {
            string mapName = root.MapName;
            SpotInfo rootInfo = mapsCache.GetRootInfo(mapName);
            root.BuildFromInfo(rootInfo, mapsCache.GetAllMapsSpotsInfo(mapName));
        }
    }

    public static void LogInfoOfListOfSpots(List<SpotInfo> spots)
    {
        foreach (SpotInfo spt in spots)
        {
            spt.LogInfo();
        }
    }

    public void OnEndOfGameClicked(Transform btnTransform)
    {
        audioRequisitor.RequestBGM(winSound);
        customPopUpOpener.Open("You Beat the game!!!", "You are Awesome!", "Go to Menu", "Look the Map", GoToMenu);
    }

    private void GoToMenu()
    {
        sceneOpener.OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    #region On Battle Spot Buttons Clicked

    public void OnSimpleBattleClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            BattleInfo.PrepareSimpleBattle();
            SetSpotInfoToClearIfPlayerWins(btnTransform);
            DeckPrototypeFactory.PrepareRandomDeckForTheEnemy();
            SetUpBattleAndOpenIt();
        }

    public void OnToughBattleClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareToughBattle();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        DeckPrototypeFactory.PrepareToughRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }

    public void OnBossBattleClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareBossBattle();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        DeckPrototypeFactory.PrepareBossRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }

    #endregion

    #region On Master Buttons Clicked

    public void OnMageMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.MAGE);
        DeckPrototypeFactory.PrepareMageDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.WARRIOR);
        DeckPrototypeFactory.PrepareWarriorDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnRougueMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.ROGUE);
        DeckPrototypeFactory.PrepareRogueDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.GUARDIAN);
        DeckPrototypeFactory.PrepareGuardianDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerWins(btnTransform);
        SetUpBattleAndOpenIt();
    }

    #endregion

    private void SetUpBattleAndOpenIt()
    {
        DeckPrototypeFactory.PrepareRandomDeckForThePlayer();
        popUpOpener.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenScene("Battle");
    }

    private void SetSpotInfoToClearIfPlayerWins(Transform spotBtnTransform)
    {
        Spot spot = GetSpotComponentInParent(spotBtnTransform);

        mapsCache.SetSpotInfoToClearIfPlayerWins(spot.gameObject.name, spot.MapName);
    }

    private Spot GetSpotComponentInParent(Transform btnTransform)
    {
        Spot spot = btnTransform.parent.GetComponent<Spot>();

        if (spot == null)
        {
            Debug.LogError("[UIMap] spot is null.", this);
        }

        return spot;
    }

    public void OnPauseMenuOppenerBtnClick()
    {
        popUpOpener.OpenPausePopUp();
    }
}
