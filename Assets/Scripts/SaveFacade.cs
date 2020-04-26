using UnityEngine;
using System.Collections;

public class SaveFacade
{
    // Needed for loading.
    private static string[] namesOfMapsToLoad;

    private MapsPersistence mapsPersistence = new MapsPersistence();
    
    // Needed for saving.
    private static string[] cachedMapNames;
    private static MapData[] cachedMapsInfo;

    // Results of loading
    private static MapData[] loadedMapsInfo;

    public bool DoesAnySaveExist()
    {
        return mapsPersistence.DoesSaveExist("First");
    }

    public void PrepareMapsForSaving(string[] mapNames, MapData[] mapsInfo)
    {
        cachedMapNames = mapNames;
        cachedMapsInfo = mapsInfo;
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        namesOfMapsToLoad = mapNames;
    }

    public void SaveEverything()
    {
        if ( SafeToSave() )
        {
            mapsPersistence.SaveAllMaps(cachedMapNames, cachedMapsInfo);
            ClassInfo.SaveClassesBonuses();
        }
        else
        {
            L.ogError("SaveEverything was called, but at least one attribute is still null.", this);
        }
    }

    private bool SafeToSave()
    {
        return cachedMapsInfo != null && cachedMapNames != null;
    }

    public void LoadEverything()
    {
        if (DoesAnySaveExist())
        {
            loadedMapsInfo = mapsPersistence.LoadAllMaps(namesOfMapsToLoad);
            ClassInfo.LoadBonusesIfAny();
        }
        else
        {
            L.ogError("LoadEverything was called, but any save exist on storage", this);
        }
    }

    public MapData[] GetLoadedMapsInfo()
    {
        return loadedMapsInfo;
    }
}
