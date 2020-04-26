using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapsCacheGetter : MapsRuntimeCache
{
    public bool DoesSaveExist()
    {
        return cache.DoesSaveExist();
    }

    public SpotInfo GetRootInfo(string mapName)
    {
        return cache.GetRootInfo(mapName);
    }

    public MapData GetMapInfo(string mapName)
    {
        return cache.GetMapInfo(mapName);
    }

    public void CacheRootsAndSpotsOfSingleMap(List<SpotInfo> allSpotsInfo, int rootIndex, string mapName)
    {
        cache.CacheRootsAndSpots(allSpotsInfo, rootIndex, mapName);
    }

    public bool DataStructuresAreEmpty()
    {
        return cache.DataStructuresAreEmpty();
    }

    public void FillMapsCacheWithSaveFilesData(string[] mapNames)
    {
       cache.FillMapsCacheWithSaveFilesData(mapNames);
    }

    public List<SpotInfo> GetSpotsInfoList(string mapName)
    {
        return cache.GetSpotsInfoList(mapName);
    }

    public void ClearLastSpotWon()
    {
        cache.ClearLastSpotWon();
    }

    public void SetSpotInfoToClearIfPlayerWins(string spotInfoGOName, string mapName)
    {
        cache.SetSpotInfoToClearIfPlayerWins(spotInfoGOName, mapName);
    }

    public void SaveAllMapsInDeviceStorage()
    {
        cache.SaveAllMapsInDeviceStorage();
    }
}
