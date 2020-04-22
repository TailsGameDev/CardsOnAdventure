using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class MapsPersistenceInstance : MapsPersistence
{
    [SerializeField]
    private Persistence percistence = null;

    private void Awake()
    {
        if (mapPersistence == null){
            mapPersistence = this;
        }
    }

    public void SaveMap(string mapName, MapInfo mapInfo)
    {
        percistence.GenericSave(mapName, mapInfo);
    }

    public bool DoSaveExists(string mapName)
    {
        return File.Exists(Application.persistentDataPath + mapName);
    }

    public MapInfo LoadMap(string mapName)
    {
        return percistence.GenericLoad<MapInfo>(mapName);
    }
}