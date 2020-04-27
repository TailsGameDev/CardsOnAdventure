using UnityEngine;
using System.Collections;

public class SaveFacade
{
    // Needed for loading.
    private static string[] namesOfMapsToLoad;

    private MapsPersistence mapsPersistence = new MapsPersistence();
    private ClassesPersistence classesPersistence = new ClassesPersistence();

    // Needed for saving.
    private static string[] nameOfMapsToSave;
    private static MapData[] dataOfMapsToSave;
    private static ClassesSerializable classesSerializableToSave;

    // Results of loading
    private static MapData[] loadedMapsInfo;
    private static ClassesSerializable loadedClassesSerializable;

    public bool DoesAnySaveExist()
    {
        return mapsPersistence.DoesSaveExist("First");
    }

    public void PrepareMapsForSaving(string[] mapNames, MapData[] mapsInfo)
    {
        nameOfMapsToSave = mapNames;
        dataOfMapsToSave = mapsInfo;
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        namesOfMapsToLoad = mapNames;
    }

    public void PrepareClassesBonusesForSaving( ClassesSerializable classesInfo )
    {
        classesSerializableToSave = classesInfo;
    }

    public void SaveEverything()
    {
        if ( SafeToSave() )
        {
            mapsPersistence.SaveAllMaps(nameOfMapsToSave, dataOfMapsToSave);
            classesPersistence.SaveClasses(classesSerializableToSave);
        }
        else
        {
            L.ogError("SaveEverything was called, but at least one attribute is still null.", this);
        }
    }

    private bool SafeToSave()
    {
        return  dataOfMapsToSave != null && 
                nameOfMapsToSave != null && 
                classesSerializableToSave != null;
    }

    public void LoadEverything()
    {
        if (DoesAnySaveExist())
        {
            loadedMapsInfo = mapsPersistence.LoadAllMaps(namesOfMapsToLoad);
            loadedClassesSerializable = classesPersistence.LoadClasses();
        }
        else
        {
            L.ogError("LoadEverything was called, but any save exist on storage", this);
        }
    }

    public MapData[] GetLoadedMapsInfo()
    {
        if (loadedMapsInfo == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedMapsInfo;
    }

    public ClassesSerializable GetLoadedClasses()
    {
        if (loadedClassesSerializable == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedClassesSerializable;
    }
}
