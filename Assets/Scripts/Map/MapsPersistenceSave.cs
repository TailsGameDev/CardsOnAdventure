using UnityEngine;
using System.Collections;

public class MapsPersistenceSave : MapsPersistence
{
    public void SaveMap(string mapName, MapInfo mapInfo)
    {
        mapPersistence.SaveMap(mapName, mapInfo);
    }
}
