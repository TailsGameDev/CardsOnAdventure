using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapsCacheGetter : MapsRuntimeCache
{
    public bool StartOfMatch { get => cache.StartOfMatch; set => cache.StartOfMatch = value; }

    public bool DoesSaveExist()
    {
        return cache.DoesSaveExist();
    }

    public SpotInfo GetRootInfo(string mapName)
    {
        return cache.GetRootInfo(mapName);
    }

    public MapInfo GetMapInfo(string mapName)
    {
        return cache.GetMapInfo(mapName);
    }

    public void ClearRootsAndSpots()
    {
        cache.ClearRootsAndSpots();
    }

    public void CacheRootsAndSpotsOfSingleMap(List<SpotInfo> allSpotsInfo, int rootIndex, string mapName)
    {
        cache.CacheRootsAndSpots(allSpotsInfo, rootIndex, mapName);
    }

    public bool DataStructuresAreEmpty()
    {
        return cache.DataStructuresAreEmpty();
    }

    public bool TryToLoadAllMapsDataFromDeviceStorage(Spot[] finalSpotForEachMap)
    {
        return cache.TryToLoadAllMapsDataFromDeviceStorage(finalSpotForEachMap);
    }

    public List<SpotInfo> GetAllMapsSpotsInfo(string mapName)
    {
        return cache.GetAllMapsSpotsInfo(mapName);
    }

    public void UpdateWithPlayerProgress()
    {
        cache.UpdateWithPlayerProgress();
    }

    public void SetSpotInfoToClearIfPlayerWins(string spotInfoGOName, string mapName)
    {
        cache.SetSpotInfoToClearIfPlayerWins(spotInfoGOName, mapName);
    }
}
