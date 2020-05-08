using System.Collections;
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
    private AudioClip bossBGM = null;
    [SerializeField]
    private AudioClip mastersBGM = null;

    [SerializeField]
    private AudioClip winSound = null;

    private MapsCache mapsCache;

    private SaveFacade saveFacade = new SaveFacade();
    #endregion

    #region Initialization
    private void Awake()
    {
        mapsCache = new MapsCacheGetter().GetCacheInstance();

        mapsCache.ClearLastSpotVisitedIfAny();

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
        customPopUpOpener.Open(
                title: "You Beat the game!!!",
                warningMessage: "You are Awesome!",
                confirmBtnMessage: "Look the Map",
                cancelBtnMessage: "Go to Menu",
                onConfirm: () => { customPopUpOpener.ClosePopUpOnTop(); },
                onCancel: sceneOpener.OpenMainMenu
            );
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
        CurrentBattleInfo.PrepareBattle(Color.white);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        DeckPrototypeFactory.PrepareRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }
    public void OnToughBattleClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Color.red);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        DeckPrototypeFactory.PrepareToughRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }
    public void OnBossBattleClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Color.black, bgmParam: bossBGM);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        DeckPrototypeFactory.PrepareBossRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }
    #endregion

    #region On Master Buttons Clicked
    public void OnMageMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Classes.MAGE, mastersBGM);
        DeckPrototypeFactory.PrepareClassDeckForTheEnemy(0, Classes.MAGE);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Classes.WARRIOR, mastersBGM);
        DeckPrototypeFactory.PrepareClassDeckForTheEnemy(0, Classes.WARRIOR);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnRougueMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Classes.ROGUE, mastersBGM);
        DeckPrototypeFactory.PrepareClassDeckForTheEnemy(0, Classes.ROGUE);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        CurrentBattleInfo.PrepareBattle(Classes.GUARDIAN, mastersBGM);
        DeckPrototypeFactory.PrepareClassDeckForTheEnemy(0, Classes.GUARDIAN);
        SetSpotInfoToClearIfPlayerSucceed(btnTransform);
        SetUpBattleAndOpenIt();
    }
    #endregion

    public void SetSpotInfoToClearIfPlayerSucceed(Spot spot)
    {
        mapsCache.SetSpotInfoToClearIfPlayerSucceed(spot.gameObject.name, spot.MapName);
    }

    public void SetSpotInfoToClearIfPlayerSucceed(Transform spotBtnTransform)
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
