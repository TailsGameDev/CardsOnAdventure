using System.Collections.Generic;
using UnityEngine;

public class MapsRuntimeCache : MonoBehaviour
{
    private static MapsCache instance;

    public static MapsCache Instance { 
        get
        {
            if (instance == null)
            {
                instance = new MapsCache();
            }
            return instance;
        }
    }

    private void Update()
    {
        Dictionary<string, Spot.SpotInfo> maps = Instance.RetrieveMapsOrGetNull();
        if (maps != null)
        {
            Spot.LogInfo(maps["First"]);
        }
    }
}

public class MapsCache
{
    private Dictionary<string, Spot.SpotInfo> mapsRuntimeCache;

    public void CacheMaps(Dictionary<string, Spot.SpotInfo> mapsRuntimeCache)
    {
        this.mapsRuntimeCache = mapsRuntimeCache;
    }

    public Dictionary<string, Spot.SpotInfo> RetrieveMapsOrGetNull()
    {
        if (mapsRuntimeCache == null)
        {
            return null;
        }
        else
        {
            return mapsRuntimeCache;
        }
    }
}
