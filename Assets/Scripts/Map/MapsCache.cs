using System.Collections.Generic;
using UnityEngine;

public class MapsCache : MapsRuntimeCache
{
    [SerializeField]
    private MapsPersistenceInstance mapsPersistence = null;

    private static int NODE_NOT_FOUND = -1;

    private Dictionary<string, SpotInfo> mapsRuntimeCache;

    private bool startOfMatch;

    private Dictionary<string, SpotInfo> mapsRoots;
    private static Dictionary<string, List<SpotInfo>> mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();

    public static SpotInfo SpotToClearIfPlayerWins;
    private string nameOfMapThatPlayerCurrentlyIs;

    public bool StartOfMatch { get => startOfMatch; set => startOfMatch = value; }
    public string NameOfMapThatPlayerCurrentlyIs { get => nameOfMapThatPlayerCurrentlyIs; set => nameOfMapThatPlayerCurrentlyIs = value; }
    public Dictionary<string, SpotInfo> MapsRoots { get => mapsRoots; set => mapsRoots = value; }

    private void Awake()
    {
        if (cache == null)
        {
            cache = this;
        }
    }

    public void CacheMaps(Dictionary<string, SpotInfo> mapsRuntimeCache)
    {
        this.mapsRuntimeCache = mapsRuntimeCache;
    }

    public Dictionary<string, SpotInfo> GetMapsRootsOrGetNull()
    {
        return mapsRuntimeCache;
    }

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

    public MapInfo GetMapInfo(string mapName)
    {
        SpotInfo root = MapsRoots[mapName];
        int rootIndex = GetIndex(root, mapName);
        return new MapInfo(mapsSpotsInfo[mapName], rootIndex);
    }

    public void ClearRootsAndSpots()
    {
        mapsRoots = new Dictionary<string, SpotInfo>();
        mapsSpotsInfo = new Dictionary<string, List<SpotInfo>>();
    }

    public void CacheRootsAndSpots(List<SpotInfo> allSpotsInfo, int rootIndex, string mapName)
    {
        mapsSpotsInfo[mapName] = allSpotsInfo;
        mapsRoots[mapName] = allSpotsInfo[rootIndex];
    }

    public bool DataStructuresAreEmpty()
    {
        return mapsRoots == null;
    }

    public bool TryToLoadAllMapsDataFromDeviceStorage(Spot[] finalSpotForEachMap)
    {
        bool succeed = true;
        mapsRoots = new Dictionary<string, SpotInfo>();
        foreach (Spot root in finalSpotForEachMap)
        {
            succeed = succeed && LoadMapDataFromDeviceStorageIfPossible(root);
        }
        return succeed;
    }

    private bool LoadMapDataFromDeviceStorageIfPossible(Spot root)
    {
        string mapName = root.MapName;
        bool saveExists = mapsPersistence.DoSaveExists(mapName);
        if (saveExists)
        {
            LoadDataFromDeviceStorage(mapName);
        }
        return saveExists;
    }

    private void LoadDataFromDeviceStorage(string mapName)
    {
        MapInfo mapInfo = mapsPersistence.LoadMap(mapName);

        mapsSpotsInfo[mapName] = mapInfo.Recover(out int rootIndex);

        SpotInfo rootInfo = mapsSpotsInfo[mapName][rootIndex];
        mapsRoots.Add(mapName, rootInfo);
    }

    public SpotInfo GetRootInfo(string mapName)
    {
        return cache.MapsRoots[mapName];
    }

    public List<SpotInfo> GetAllMapsSpotsInfo(string mapName)
    {
        return mapsSpotsInfo[mapName];
    }

    public void UpdateWithPlayerProgress()
    {
        if (SpotToClearIfPlayerWins != null)
        {
            SpotToClearIfPlayerWins.Cleared = true;
            SaveMapInDeviceStorage(nameOfMapThatPlayerCurrentlyIs);
        }
    }

    private void SaveMapInDeviceStorage(string mapName)
    {
        mapsPersistence.SaveMap(mapName, GetMapInfo(mapName));
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
}
