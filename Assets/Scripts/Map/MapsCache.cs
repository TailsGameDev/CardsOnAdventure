using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapsCache : MapsRuntimeCache
{
    private static int NODE_NOT_FOUND = -1;

    private SaveFacade saveFacade = new SaveFacade();

    private Dictionary<string, SpotInfo> mapsRoots = new Dictionary<string, SpotInfo>();
    private static Dictionary<string, List<SpotInfo>> mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();

    public static SpotInfo SpotToClearIfPlayerWins;

    #region Get Some Info
    public SpotInfo GetRootInfo(string mapName)
    {
        return mapsRoots[mapName];
    }
    public SpotInfo GetInfoFromGraphOrGetNull(string desiredInfoGOName, string mapName)
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
    public List<SpotInfo> GetSpotsInfoList(string mapName)
    {
        return mapsSpotsInfo[mapName];
    }
    #endregion

    public void CacheRootsAndSpotsOfSingleMap(List<SpotInfo> allSpotsInfo, int rootIndex, string mapName)
    {
        mapsSpotsInfo[mapName] = allSpotsInfo;
        mapsRoots[mapName] = allSpotsInfo[rootIndex];
    }

    public bool DataStructuresAreEmpty()
    {
        return mapsRoots.Keys.Count == 0 && mapsSpotsInfo.Keys.Count == 0;
    }

    public void SetSpotInfoToClearIfPlayerSucceed(string spotInfoGOName, string mapName)
    {
        SpotToClearIfPlayerWins = GetInfoFromGraphOrGetNull(spotInfoGOName, mapName);
    }
    public void ClearLastSpotVisitedIfAny()
    {
        if (SpotToClearIfPlayerWins != null)
        {
            SpotToClearIfPlayerWins.Cleared = true;
        }
    }

    #region Deals with saveFacade
    public void PrepareAllMapsForSaving()
    {
        string[] mapNames = GetMapNames();

        int mapsAmount = mapNames.Length;

        // Get All Maps Info
        MapData[] mapsInfo = new MapData[mapsAmount];
        for (int i = 0; i < mapsAmount; i++)
        {
            mapsInfo[i] = GetInfo(mapName: mapNames[i]);
        }

        saveFacade.PrepareMapsForSaving(mapNames, mapsInfo);
    }
    private string[] GetMapNames()
    {
        return mapsRoots.Keys.ToArray();
    }
    private MapData GetInfo(string mapName)
    {
        List<SpotInfo> spotsInfo = mapsSpotsInfo[mapName];

        SpotInfo root = mapsRoots[mapName];
        int rootIndex = GetIndex(root, mapName);

        return new MapData(spotsInfo, rootIndex);
    }
    private int GetIndex(SpotInfo desiredInfo, string mapName)
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

    /*
    public bool DoesSaveExist()
    {
        return saveFacade.DoesAnySaveExist();
    }
    */

    public void PrepareMapsForLoading(string[] mapNames)
    {
        saveFacade.PrepareMapsForLoading(mapNames);
    }

    public void FillMapsCacheUsingLoadedFiles(string[] mapNames)
    {
        MapData[] mapsData = saveFacade.GetLoadedMapsInfo();

        for (int i = 0; i < mapsData.Length; i++)
        {
            CopyMapDataToAttributes( mapNames[i], mapsData[i] );
        }
    }
    private void CopyMapDataToAttributes(string mapName, MapData mapsData)
    {
        mapsSpotsInfo[mapName] = mapsData.Recover(out int rootIndex);

        // Get root
        List<SpotInfo> spotsInfo = mapsSpotsInfo[mapName];
        SpotInfo rootInfo = spotsInfo[rootIndex];

        mapsRoots[mapName] = rootInfo;
    }
    #endregion
}
