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

    /*
    private void Update()
    {
        Dictionary<string, SpotInfo> maps = Instance.RetrieveMapsOrGetNull();
        if (maps != null)
        {
            maps["First"].LogInfo();
        }
        else
        {
            L.ogError("maps[First] is null att MapsRuntimeCache.", this);
        }
    }
    */
}

public class MapsCache
{
    private Dictionary<string, SpotInfo> mapsRuntimeCache;

    public void CacheMaps(Dictionary<string, SpotInfo> mapsRuntimeCache)
    {
        this.mapsRuntimeCache = mapsRuntimeCache;
    }

    public Dictionary<string, SpotInfo> RetrieveMapsOrGetNull()
    {
        return mapsRuntimeCache;
    }
}
