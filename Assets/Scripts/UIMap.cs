using System.Collections.Generic;
using UnityEngine;

public class UIMap : PopUpOpener
{
    private static int NODE_NOT_FOUND = -1;

    [SerializeField]
    private Spot[] finalSpotForEachMap = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip winSound = null;

    [SerializeField]
    private MapPersistence mapPersistence = null;
    private static bool startOfMatch;

    public static bool StartOfMatch { set => startOfMatch = value; }

    private Dictionary<string, SpotInfo> mapsRoots;
    private static Dictionary<string, List<SpotInfo>> mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();

    public static SpotInfo spotToClearIfPlayerWins;
    private static string nameOfMapThatPlayerCurrentlyIs;

    private void Awake()
    {
        mapsRoots = MapsRuntimeCache.Instance.RetrieveMapsOrGetNull();

        if (spotToClearIfPlayerWins != null)
        {
            spotToClearIfPlayerWins.Cleared = true;
            SaveInDeviceStorage(spotToClearIfPlayerWins, nameOfMapThatPlayerCurrentlyIs);
        }

        if (startOfMatch)
        {
            BuildAllMaps();
        }
        else if (mapsRoots == null)
        {
            bool allSavesFound = true;
            mapsRoots = new Dictionary<string, SpotInfo>();
            foreach (Spot root in finalSpotForEachMap)
            {
                string mapName = root.MapName;
                if (mapPersistence.DoSaveExists(mapName))
                {
                    MapInfo mapInfo = mapPersistence.LoadMap(mapName);

                    mapsSpotsInfo[mapName] = mapInfo.Recover(out int rootIndex);

                    SpotInfo rootInfo = mapsSpotsInfo[mapName][rootIndex];
                    mapsRoots.Add(mapName, rootInfo);
                }
                else
                {
                    allSavesFound = false;
                }
            }

            if (allSavesFound)
            {
                RecoverSavedMaps();
            }
            else
            {
                BuildAllMaps();
            }
        }
        else
        {
            RecoverSavedMaps();
        }

        MapsRuntimeCache.Instance.CacheMaps(mapsRoots);
    }

    private void SaveInDeviceStorage(SpotInfo spot, string mapName)
    {
        SpotInfo rootInfo = mapsRoots[mapName];
        int rootIndex = GetIndex(rootInfo, mapName);

        MapInfo mapInfo = new MapInfo(mapsSpotsInfo[mapName], rootIndex);

        mapPersistence.SaveMap(mapName, mapInfo);
    }

    /*
    private Spot FindSpotInRootsOrGetNull(string mapName)
    {
        foreach (Spot spot in finalSpotForEachMap)
        {
            if (spot.MapName == mapName)
            {
                return spot;
            }
        }
        return null;
    }
    */

    public int GetIndex(SpotInfo desiredInfo, string mapName)
    {
        List<SpotInfo> allSpotsInfo = mapsSpotsInfo[mapName];

        for (int i = 0; i < allSpotsInfo.Count; i++)
        {
            if (desiredInfo == allSpotsInfo[i])
            {
                return i;
            }
        }
        return NODE_NOT_FOUND;
    }

    public SpotInfo GetInfoFromGaphOrGetNull(string desiredInfoGOName, string mapName)
    {
        List<SpotInfo> allSpotsInfo = mapsSpotsInfo[mapName];

        for (int i = 0; i < allSpotsInfo.Count; i++)
        {
            if (desiredInfoGOName == allSpotsInfo[i].GOName)
            {
                return allSpotsInfo[i];
            }
        }
        return null;
    }

    /*
    private void Update()
    {
        LogInfoOfListOfSpots(mapsSpotsInfo["First"]);
    }
    */

    public static void LogInfoOfListOfSpots(List<SpotInfo> spots)
    {
        foreach (SpotInfo spt in spots)
        {
            spt.LogInfo();
        }
    }

    private void BuildAllMaps()
    {
        startOfMatch = false;
        mapsRoots = new Dictionary<string, SpotInfo>();
        mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();

        foreach (Spot mapFinalSpot in finalSpotForEachMap)
        {
            BuildMapAndSaveInCollection(mapFinalSpot);
        }
    }

    private void BuildMapAndSaveInCollection(Spot mapFinalSpot)
    {
        string mapName = mapFinalSpot.MapName;
        mapFinalSpot.BuildFromZero();
        List<SpotInfo> allSpotsInfo = mapFinalSpot.GetInfo(out int rootIndex);
        mapsSpotsInfo[mapName] = allSpotsInfo;
        mapsRoots[mapName] = allSpotsInfo[rootIndex];
    }

    public void RecoverSavedMaps()
    {
        foreach (Spot root in finalSpotForEachMap)
        {
            string mapName = root.MapName;
            SpotInfo rootInfo = mapsRoots[mapName];
            root.BuildFromInfo(rootInfo, mapsSpotsInfo[mapName]);
        }
    }

    void GoToMenu()
    {
        sceneOpener.OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    public void OnEndOfGameClicked(Transform btnTransform)
    {
        audioRequisitor.RequestBGM(winSound);
        customPopUpOpener.Open("You Beat the game!!!", "You are Awesome!", "Go to Menu", "Look the Map", GoToMenu);
        //ClearSpot(btnTransform);
        //BattleDefaultBehaviour();
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

    // the btnTransform parameter might be used if we substitute the final spot for some battle
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
        Spot spot = FindAndClearSpotInItsGameObject(spotBtnTransform);

        spotToClearIfPlayerWins = GetInfoFromGaphOrGetNull(spot.gameObject.name, spot.MapName);
        nameOfMapThatPlayerCurrentlyIs = spot.MapName;
    }
    /*
    private void ClearSpotAndSaveInfo(Transform spotBtnTransform)
    {
        Spot spot = FindAndClearSpotInItsGameObject(spotBtnTransform);

        spot.Cleared = true;

        SetClearedInMapsCache(spot);

        SaveInDeviceStorage(spot);
    }*/

    private Spot FindAndClearSpotInItsGameObject(Transform btnTransform)
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
