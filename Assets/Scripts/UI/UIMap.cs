using System.Collections.Generic;
using UnityEngine;

public class UIMap : OpenersSuperclass
{
    #region Attributes
    public static bool StartOfMatch = true;

    [SerializeField]
    private Spot[] finalSpotForEachMap = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip winSound = null;

    private MapsCache mapsCache;

    private SaveFacade saveFacade = new SaveFacade();
    #endregion

    #region Initialization
    private void Awake()
    {
        mapsCache = new MapsCacheGetter().GetCacheInstance();

        mapsCache.ClearLastSpotVisited();

        if (StartOfMatch)
        {
            ClassInfo.ResetBonusesToAllClasses();
            StartOfMatch = false;
            BuildSpotsFromZeroThenCacheThem();
        }
        else if ( CacheIsEmpty() )
        {
            InitializeGameDataConsideringStorage();
        }
        else
        {
            CopyDataFromMapsCacheToSceneSpots();
        }

        ClassInfo.PrepareClassesBonusesForSaving();
        mapsCache.PrepareAllMapsForSaving();
        saveFacade.SaveEverything();
    }

    private void BuildSpotsFromZeroThenCacheThem()
    {
        foreach (Spot mapFinalSpot in finalSpotForEachMap)
        {
            mapFinalSpot.BuildFromZero();
        }

        foreach (Spot mapFinalSpot in finalSpotForEachMap)
        {
            string mapName = mapFinalSpot.MapName;
            List<SpotInfo> allSpotsInfo = mapFinalSpot.GetInfo(out int rootIndex);

            mapsCache.CacheRootsAndSpotsOfSingleMap(allSpotsInfo, rootIndex, mapName);
        }
    }

    private bool CacheIsEmpty()
    {
        return mapsCache.DataStructuresAreEmpty();
    }

    private void InitializeGameDataConsideringStorage()
    {
        if (saveFacade.DoesAnySaveExist())
        {
            BringAllDataFromStorage();
        }
        else
        {
            // Build the data from the scene and then save into the cache.
            BuildSpotsFromZeroThenCacheThem();
            // Default class Information should be already set in ClassInfo.
        }
    }
    private void BringAllDataFromStorage()
    {
        // Preparing for load
        string[] mapNames = GetMapNames();
        mapsCache.PrepareMapsForLoading(mapNames);

        saveFacade.LoadEverything();

        // Copying loaded information into the game classes.
        mapsCache.FillMapsCacheUsingLoadedFiles(mapNames);
        CopyDataFromMapsCacheToSceneSpots();
        ClassInfo.CopyLoadedClassesToAttributes();
    }
    private string[] GetMapNames()
    {
        int amountOfMaps = finalSpotForEachMap.Length;
        string[] names = new string[amountOfMaps];
        for (int i = 0; i < amountOfMaps; i++)
        {
            names[i] = finalSpotForEachMap[i].MapName;
        }
        return names;
    }

    public void CopyDataFromMapsCacheToSceneSpots()
    {
        foreach (Spot root in finalSpotForEachMap)
        {
            string mapName = root.MapName;
            SpotInfo rootInfo = mapsCache.GetRootInfo(mapName);
            root.BuildFromInfo(rootInfo, mapsCache.GetSpotsInfoList(mapName));
        }
    }
    #endregion

    public void OnPauseMenuOppenerBtnClick()
    {
        openerOfPopUpsMadeInEditor.OpenPausePopUp();
    }

    #region On "Some Not Battle Spot" Clicked
    public void OnEndOfGameClicked(Transform btnTransform)
    {
        audioRequisitor.RequestBGM(winSound);
        customPopUpOpener.Open("You Beat the game!!!", "You are Awesome!", "Go to Menu", "Look the Map", sceneOpener.OpenMainMenu);
    }
    public void OnDeckBuildBtnClicked(Transform btnTransform)
    {
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        sceneOpener.OpenDeckBuildingScene();
    }
    #endregion

    #region On Battle Spot Buttons Clicked
    public void OnSimpleBattleClicked(Transform btnTransform)
        {
            openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
            BattleInfo.PrepareSimpleBattle();
            SetSpotInfoToClearIfPlayerSucceed(btnTransform);
            DeckPrototypeFactory.PrepareRandomDeckForTheEnemy();
            SetUpBattleAndOpenIt();
        }
    public void OnToughBattleClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareToughBattle();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        DeckPrototypeFactory.PrepareToughRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }
    public void OnBossBattleClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareBossBattle();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        DeckPrototypeFactory.PrepareBossRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }
    #endregion

    #region On Master Buttons Clicked
    public void OnMageMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.MAGE);
        DeckPrototypeFactory.PrepareMageDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.WARRIOR);
        DeckPrototypeFactory.PrepareWarriorDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnRougueMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.ROGUE);
        DeckPrototypeFactory.PrepareRogueDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.GUARDIAN);
        DeckPrototypeFactory.PrepareGuardianDeckForTheEnemy();
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    #endregion

    private void SetSpotInfoToClearIfPlayerSucceed(Transform spotBtnTransform)
    {
        Spot spot = GetSpotComponentInParent(spotBtnTransform);

        mapsCache.SetSpotInfoToClearIfPlayerSucceed(spot.gameObject.name, spot.MapName);
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
    private void SetUpBattleAndOpenIt()
    {
        sceneOpener.OpenBattle();
    }

    private class MapsCacheGetter : MapsRuntimeCache
    {
        public MapsCache GetCacheInstance()
        {
            return cache;
        }
    }

    public static void LogInfoOfListOfSpots(List<SpotInfo> spots)
    {
        foreach (SpotInfo spt in spots)
        {
            spt.LogInfo();
        }
    }
}
