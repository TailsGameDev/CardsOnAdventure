public class MapsPersistence : Persistence
{
    // Inherit GenericSave, DoesSaveExist, GenericLoad

    public void SaveAllMaps(string[] mapNames, MapSerializable[] mapsInfo)
    {
        for (int i = 0; i < mapNames.Length; i++)
        {
            GenericSave(mapNames[i], mapsInfo[i]);
        }
    }

    public bool DoesMapSaveExist(string mapName)
    {
        return DoesSaveExist(mapName);
    }

    public MapSerializable[] LoadAllMaps(string[] mapNames)
    {
        MapSerializable[] mapsInfo = new MapSerializable[mapNames.Length];

        for (int i = 0; i < mapNames.Length; i++)
        {
            mapsInfo[i] = GenericLoad<MapSerializable>(mapNames[i]);
        }

        return mapsInfo;
    }
}