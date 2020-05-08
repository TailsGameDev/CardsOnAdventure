using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : OpenersSuperclass
{    
    public static bool StartOfMatch = true;

    [SerializeField]
    private Spot[] finalSpotForEachMap = null;

    private MapsCache mapsCache;

    private SaveFacade saveFacade = new SaveFacade();

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

    public void SetSpotInfoToClearIfPlayerSucceed(Spot spot)
    {
        mapsCache.SetSpotInfoToClearIfPlayerSucceed(spot.gameObject.name, spot.MapName);
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
