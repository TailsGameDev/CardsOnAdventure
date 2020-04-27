using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapsCache : MapsRuntimeCache
{
    private static int NODE_NOT_FOUND = -1;

    private SaveFacade saveFacade = new SaveFacade();

    private Dictionary<string, SpotInfo> mapsRuntimeCache;

    private Dictionary<string, SpotInfo> mapsRoots = new Dictionary<string, SpotInfo>();
    private static Dictionary<string, List<SpotInfo>> mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();

    public static SpotInfo SpotToClearIfPlayerWins;
    private string nameOfMapThatPlayerCurrentlyIs;

    public string NameOfMapThatPlayerCurrentlyIs { set => nameOfMapThatPlayerCurrentlyIs = value; }
    public Dictionary<string, SpotInfo> MapsRoots { get => mapsRoots; set => mapsRoots = value; }

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

    public SpotInfo GetRootInfo(string mapName)
    {
        return mapsRoots[mapName];
    }

    public MapData GetMapInfo(string mapName)
    {
        SpotInfo root = mapsRoots[mapName];
        int rootIndex = GetIndex(root, mapName);
        return new MapData(mapsSpotsInfo[mapName], rootIndex);
    }

    public void CacheRootsAndSpots(List<SpotInfo> allSpotsInfo, int rootIndex, string mapName)
    {
        mapsSpotsInfo[mapName] = allSpotsInfo;
        mapsRoots[mapName] = allSpotsInfo[rootIndex];
    }

    public bool DataStructuresAreEmpty()
    {
        return mapsRoots.Keys.Count == 0 && mapsSpotsInfo.Keys.Count == 0;
    }

    public List<SpotInfo> GetSpotsInfoList(string mapName)
    {
        return mapsSpotsInfo[mapName];
    }

    public void ClearLastSpotWon()
    {
        if (SpotToClearIfPlayerWins != null)
        {
            SpotToClearIfPlayerWins.Cleared = true;
        }
    }

    public void SetSpotInfoToClearIfPlayerWins(string spotInfoGOName, string mapName)
    {
        SpotToClearIfPlayerWins = GetInfoFromGraphOrGetNull(spotInfoGOName, mapName);
        NameOfMapThatPlayerCurrentlyIs = mapName;
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

    #region Deals with saveFacade
    public void PrepareAllMapsForSaving()
    {
        string[] mapNames = GetMapNames();

        int mapsAmount = mapNames.Length;

        // Get Maps Info
        MapData[] mapsInfo = new MapData[mapsAmount];
        for (int i = 0; i < mapsAmount; i++)
        {
            mapsInfo[i] = GetInfo(mapName: mapNames[i]);
        }

        saveFacade.PrepareMapsForSaving(mapNames, mapsInfo);
    }

    private MapData GetInfo(string mapName)
    {
        List<SpotInfo> spotsInfo = mapsSpotsInfo[mapName];

        SpotInfo root = mapsRoots[mapName];
        int rootIndex = GetIndex(root, mapName);

        return new MapData(spotsInfo, rootIndex);
    }

    public bool DoesSaveExist()
    {
        return saveFacade.DoesAnySaveExist();
    }

    private string[] GetMapNames()
    {
        return mapsRoots.Keys.ToArray();
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        saveFacade.PrepareMapsForLoading(mapNames);
    }

    public void FillMapsCacheWithLoadedFiles(string[] mapNames)
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
