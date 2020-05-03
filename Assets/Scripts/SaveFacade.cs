using UnityEngine;
using System.Collections;

public class SaveFacade
{
    private MapsPersistence mapsPersistence = new MapsPersistence();
    private ClassesPersistence classesPersistence = new ClassesPersistence();
    private DeckPersistence deckPersistence = new DeckPersistence();

    // Needed for saving.
    private static string[] nameOfMapsToSave;
    private static MapData[] dataOfMapsToSave;
    private static ClassesSerializable classesSerializableToSave;
    private static DeckSerializable deckSerializableToSave;

    // Needed for loading.
    private static string[] namesOfMapsToLoad;

    // Results of loading
    private static MapData[] loadedMapsInfo;
    private static ClassesSerializable loadedClassesSerializable;
    private static DeckSerializable loadedDeckSerializable;

    public bool DoesAnySaveExist()
    {
        return mapsPersistence.DoesMapSaveExist("First");
    }

    public void PrepareMapsForSaving(string[] mapNames, MapData[] mapsInfo)
    {
        nameOfMapsToSave = mapNames;
        dataOfMapsToSave = mapsInfo;
    }
    public void PrepareClassesBonusesForSaving( ClassesSerializable classesInfo )
    {
        classesSerializableToSave = classesInfo;
    }
    public void PrepareDeckForSaving(DeckSerializable deckSerializableParam)
    {
        deckSerializableToSave = deckSerializableParam;
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        namesOfMapsToLoad = mapNames;
    }

    public void LoadEverything()
    {
        if (DoesAnySaveExist())
        {
            loadedMapsInfo = mapsPersistence.LoadAllMaps(namesOfMapsToLoad);
            loadedClassesSerializable = classesPersistence.LoadClasses();
            loadedDeckSerializable = deckPersistence.Load();

            
            classesSerializableToSave = loadedClassesSerializable;
            deckSerializableToSave = loadedDeckSerializable;
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
    public DeckSerializable GetLoadedDeck()
    {
        return loadedDeckSerializable;
    }

    public bool IsDeckLoaded()
    {
        return loadedDeckSerializable != null;
    }

    private bool SafeToSave()
    {
        return  dataOfMapsToSave != null && 
                nameOfMapsToSave != null && 
                classesSerializableToSave != null &&
                deckSerializableToSave != null;
    }
    public void SaveEverything()
    {
        if ( SafeToSave() )
        {
            mapsPersistence.SaveAllMaps(nameOfMapsToSave, dataOfMapsToSave);
            classesPersistence.SaveClasses(classesSerializableToSave);
            deckPersistence.Save(deckSerializableToSave);
        }
        else
        {
            L.ogError("SaveEverything was called, but at least one attribute is still null.", this);
        }
    }
}
