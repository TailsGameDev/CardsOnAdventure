public class MapsPersistence : Persistence
{
    // Inherit GenericSave, DoesSaveExist, GenericLoad

    public void SaveAllMaps(string[] mapNames, MapData[] mapsInfo)
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

    public MapData[] LoadAllMaps(string[] mapNames)
    {
        MapData[] mapsInfo = new MapData[mapNames.Length];

        for (int i = 0; i < mapNames.Length; i++)
        {
            mapsInfo[i] = GenericLoad<MapData>(mapNames[i]);
        }

        return mapsInfo;
    }
}