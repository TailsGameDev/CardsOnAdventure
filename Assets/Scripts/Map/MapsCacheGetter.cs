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
       cache.FillMapsCacheWithLoadedFiles(mapNames);
    }

    public List<SpotInfo> GetSpotsInfoList(string mapName)
    {
        return cache.GetSpotsInfoList(mapName);
    }

    public void ClearLastSpotVisited()
    {
        cache.ClearLastSpotVisited();
    }

    public void SetSpotInfoToClearIfPlayerSucceed(string spotInfoGOName, string mapName)
    {
        cache.SetSpotInfoToClearIfPlayerWins(spotInfoGOName, mapName);
    }

    public void PrepareAllMapsForSaving()
    {
        cache.PrepareAllMapsForSaving();
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        cache.PrepareMapsForLoading(mapNames);
    }
}
